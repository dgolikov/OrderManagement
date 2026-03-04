using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderManagement.Application.Products;
using OrderManagement.Application.Users;
using OrderManagement.Authentication;
using OrderManagement.Domain.Common.Services;
using OrderManagement.Domain.Order;
using OrderManagement.Domain.Product;
using OrderManagement.Domain.User;
using OrderManagement.Persistanse;
using OrderManagement.WebAPI.Endpoints;
using OrderManagement.WebAPI.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.AddScoped(typeof(IMongoDatabase), sp =>
{
    var options = sp.GetRequiredService<IOptions<DatabaseOptions>>();
    var url = MongoUrl.Create(options.Value.ConnectionString);
    var client = new MongoClient(url);
    return client.GetDatabase("OrderManagement");
});


builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddScoped<IHashService, HashService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapProductEndpoints();
app.MapUserEndpoints();

app.Run();