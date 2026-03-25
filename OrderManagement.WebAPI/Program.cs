using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using OrderManagement.Application.Authentication;
using OrderManagement.Application.Orders;
using OrderManagement.Application.Products;
using OrderManagement.Application.Users;
using OrderManagement.Authentication;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Order;
using OrderManagement.Domain.Product;
using OrderManagement.Domain.Tokens;
using OrderManagement.Domain.User;
using OrderManagement.Persistence;
using OrderManagement.WebAPI.Endpoints;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

builder.Services.AddScoped(typeof(IMongoDatabase), sp =>
{
    var options = sp.GetRequiredService<IOptions<DatabaseOptions>>();
    var url = MongoUrl.Create(options.Value.ConnectionString);
    var client = new MongoClient(url);
    return client.GetDatabase("OrderManagement");
});


builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<IJwtFactory, JwtFactory>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddOpenApi();

builder.Services.AddCors();

builder.Services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
{
    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtOptions:JwtIssuer"],
        ValidAudience = builder.Configuration["JwtOptions:JwtAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:JwtSecret"] ?? ""))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors((policyBuilder) =>
{
    policyBuilder.WithOrigins("http://localhost:4200");
    policyBuilder.AllowAnyHeader();
    policyBuilder.AllowAnyMethod();
    policyBuilder.AllowCredentials();
});

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();
app.MapAuthenticationEndpoints();

app.Run();