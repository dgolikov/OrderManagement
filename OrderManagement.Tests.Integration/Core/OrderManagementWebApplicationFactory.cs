using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderManagement.Domain.Common.Options;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace OrderManagement.Tests.Integration.Core;

public class OrderManagementWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private IMongoDatabase _database = default!;
    public HttpClient HttpClient { get; private set; } = default!;
    private IServiceProvider _services = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IMongoDatabase>();

            services.AddSingleton(typeof(IMongoDatabase), _ => _database);

            services.AddSingleton<IStartupFilter, IntegrationTestsFilter>();

            services.Configure<RedisOptions>(options =>
                options.ConnectionString = _redisContainer.GetConnectionString());

            // Re-register Redis cache with test container connection string
            services.RemoveAll<RedisCache>();
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = _redisContainer.GetConnectionString());
        });
    }

    public T GetRequiredService<T>() where T : notnull => _services.GetRequiredService<T>();

    public async Task FillCollection<T>(string collectionName, IEnumerable<T> items)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.InsertManyAsync(items);
    }

    public async Task ResetDatabaseAsync()
    {
        var collections = await _database.ListCollectionNamesAsync();
        await Parallel.ForEachAsync(collections.ToEnumerable(), async (name, cancellationToken) =>
        {
            await _database.DropCollectionAsync(name, cancellationToken: cancellationToken);
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
        await _redisContainer.StartAsync();

        var url = MongoUrl.Create(_mongoDbContainer.GetConnectionString());
        var client = new MongoClient(url);
        _database = client.GetDatabase("OrderManagement");

        HttpClient = CreateClient();
        _services = Services;
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
