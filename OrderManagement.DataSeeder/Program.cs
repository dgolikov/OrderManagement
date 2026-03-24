using Bogus;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MongoDB.Driver;
using OrderManagement.Domain.Order;
using OrderManagement.Domain.Product;
using OrderManagement.Domain.User;
using System.Security.Cryptography;

namespace OrderManagement.DataSeeder;

static class Program
{
    private const int ProductCount = 1000;
    private const string ConnectionString = "mongodb://ordermnguser:12345678Aa@localhost:29700/OrderManagement?authSource=admin";
    private const string DatabaseName = "OrderManagement";
    private const int UserCount = 3;
    private const int OrdersPerUser = 300;
    private const string ProductsCollectionName = "products";
    private const string UsersCollectionName = "users";
    private const string OrdersCollectionName = "orders";

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== Order Management Test Data Seeder ===");
        Console.WriteLine();

        try
        {
            // Step 1: Seed Products
            var products = await SeedProductsAsync();
            if (products == null)
                return 1;

            // Step 2: Seed Users
            var users = await SeedUsersAsync();
            if (users == null)
                return 1;

            // Step 3: Seed Orders
            var orders = await SeedOrdersAsync(products, users);
            if (orders == null)
                return 1;

            Console.WriteLine();
            Console.WriteLine("=== Seeding Complete ===");
            Console.WriteLine($"Products: {products.Count}");
            Console.WriteLine($"Users: {users.Count}");
            Console.WriteLine($"Orders: {orders.Count}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1;
        }
    }

    private static async Task<List<Product>?> SeedProductsAsync()
    {
        Console.WriteLine("--- Products ---");

        var collection = GetMongoCollection<Product>(ProductsCollectionName);
        var existingCount = await collection.CountDocumentsAsync(FilterDefinition<Product>.Empty);

        if (existingCount >= ProductCount)
        {
            Console.WriteLine($"Found {existingCount} existing products.");
            Console.Write("Reseed products? (y/N): ");
            var response = Console.ReadLine();
            if (response?.ToLower() != "y")
            {
                Console.WriteLine("Using existing products.");
                return await collection.Find(FilterDefinition<Product>.Empty).ToListAsync();
            }
        }

        Console.WriteLine($"Generating {ProductCount} products...");

        var imageUrls = await ReadImageUrlsAsync();
        Console.WriteLine($"Loaded {imageUrls.Count} image URLs from links.txt");

        var productFaker = CreateProductFaker(imageUrls);
        var products = productFaker.Generate(ProductCount);
        Console.WriteLine($"Generated {products.Count} products");

        Console.WriteLine("Connected to MongoDB");
        await collection.DeleteManyAsync(FilterDefinition<Product>.Empty);
        await collection.InsertManyAsync(products);
        Console.WriteLine($"Successfully inserted {products.Count} products!");

        return products;
    }

    private static async Task<List<User>?> SeedUsersAsync()
    {
        Console.WriteLine();
        Console.WriteLine("--- Users ---");

        var collection = GetMongoCollection<User>(UsersCollectionName);
        var existingCount = await collection.CountDocumentsAsync(FilterDefinition<User>.Empty);

        if (existingCount >= UserCount)
        {
            Console.WriteLine($"Found {existingCount} existing users.");
            Console.Write("Reseed users? (y/N): ");
            var response = Console.ReadLine();
            if (response?.ToLower() != "y")
            {
                Console.WriteLine("Using existing users.");
                return await collection.Find(FilterDefinition<User>.Empty).ToListAsync();
            }
        }

        Console.WriteLine($"Generating {UserCount} test users (password: 12345678)...");

        var userFaker = CreateUserFaker();
        var users = userFaker.Generate(UserCount);
        Console.WriteLine($"Generated {users.Count} users");

        await collection.DeleteManyAsync(FilterDefinition<User>.Empty);
        await collection.InsertManyAsync(users);
        Console.WriteLine($"Successfully inserted {users.Count} users!");
        Console.WriteLine("User emails: test1@test.com, test2@test.com, test3@test.com");

        return users;
    }

    private static async Task<List<Order>?> SeedOrdersAsync(
        List<Product> products,
        List<User> users)
    {
        Console.WriteLine();
        Console.WriteLine("--- Orders ---");

        var collection = GetMongoCollection<Order>(OrdersCollectionName);
        var existingCount = await collection.CountDocumentsAsync(FilterDefinition<Order>.Empty);

        var totalOrdersToGenerate = users.Count * OrdersPerUser;

        if (existingCount >= totalOrdersToGenerate)
        {
            Console.WriteLine($"Found {existingCount} existing orders.");
            Console.Write("Reseed orders? (y/N): ");
            var response = Console.ReadLine();
            if (response?.ToLower() != "y")
            {
                Console.WriteLine("Using existing orders.");
                return await collection.Find(FilterDefinition<Order>.Empty).ToListAsync();
            }
        }

        Console.WriteLine($"Generating {totalOrdersToGenerate} orders ({OrdersPerUser} per user)...");

        var allOrders = new List<Order>();

        foreach (var user in users)
        {
            Console.WriteLine($"  Generating orders for {user.Email}...");
            var orderFaker = CreateOrderFaker(products, user.Id);
            var userOrders = orderFaker.Generate(OrdersPerUser);
            allOrders.AddRange(userOrders);
        }

        Console.WriteLine($"Generated {allOrders.Count} total orders");

        await collection.DeleteManyAsync(FilterDefinition<Order>.Empty);
        await collection.InsertManyAsync(allOrders);
        Console.WriteLine($"Successfully inserted {allOrders.Count} orders!");

        return allOrders;
    }

    private static async Task<List<string>> ReadImageUrlsAsync()
    {
        var executableDir = AppContext.BaseDirectory;
        var solutionRoot = Path.GetFullPath(Path.Combine(executableDir, "..", "..", "..", ".."));
        var linksPath = Path.Combine(solutionRoot, "links.txt");

        if (!File.Exists(linksPath))
        {
            throw new FileNotFoundException($"links.txt not found at: {linksPath}");
        }

        var lines = await File.ReadAllLinesAsync(linksPath);
        return lines
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }

    private static Faker<Product> CreateProductFaker(List<string> imageUrls)
    {
        var random = new Random();
        var usedSkus = new HashSet<string>();

        return new Faker<Product>()
            .CustomInstantiator(f =>
            {
                string sku;
                do
                {
                    sku = $"SKU-{f.Random.AlphaNumeric(8).ToUpper()}";
                } while (usedSkus.Contains(sku));
                usedSkus.Add(sku);

                var price = Math.Round(f.Random.Decimal(1, 999999), 2);

                var name = GenerateProductName(f);

                var imageUrl = imageUrls.Count > 0
                    ? imageUrls[f.Random.Int(0, imageUrls.Count - 1)]
                    : null;

                var result = Product.Create(name, price, sku, imageUrl);

                if (result.IsFailure)
                {
                    throw new InvalidOperationException($"Failed to create product: {result.Error?.Message}");
                }

                return result.Value;
            });
    }

    private static string GenerateProductName(Faker f)
    {
        var categories = new[] { "Pro", "Elite", "Premium", "Standard", "Basic", "Advanced", "Ultra", "Max" };
        var products = new[] { "Widget", "Gadget", "Device", "Tool", "Kit", "Set", "Pack", "Unit", "Module", "Component" };

        return $"{f.PickRandom(categories)} {f.PickRandom(products)} {f.Commerce.ProductAdjective()} {f.Random.Int(100, 9999)}";
    }

    private static string GenerateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    private static string CalculatePasswordHash(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hash = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 10000, 64)
        );
        return hash;
    }

    private static Faker<User> CreateUserFaker()
    {
        return new Faker<User>()
            .CustomInstantiator(f =>
            {
                var index = f.IndexFaker + 1;
                var firstName = $"Test{index}";
                var lastName = "User";
                var email = $"test{index}@test.com";
                var salt = GenerateSalt();
                var passwordHash = CalculatePasswordHash("12345678", salt);

                var result = Domain.User.User.Create(firstName, lastName, email, passwordHash, salt);

                if (result.IsFailure)
                {
                    throw new InvalidOperationException($"Failed to create user: {result.Error?.Message}");
                }

                return result.Value;
            });
    }

    private static Faker<Order> CreateOrderFaker(
        List<Product> products,
        Guid userId)
    {
        return new Faker<Order>()
            .CustomInstantiator(f =>
            {
                var itemCount = f.Random.Int(1, 10);
                var selectedProducts = f.PickRandom(products, itemCount).ToList();

                var lineItems = selectedProducts.Select(p =>
                    new OrderLineItem(
                        p.Id,
                        f.Random.Int(1, 5),
                        p.Price
                    )
                ).ToList();

                var result = Order.Create(userId, lineItems);

                if (result.IsFailure)
                {
                    throw new InvalidOperationException($"Failed to create order: {result.Error?.Message}");
                }

                return result.Value;
            });
    }

    private static IMongoCollection<T> GetMongoCollection<T>(string collectionName)
    {
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase(DatabaseName);
        return database.GetCollection<T>(collectionName);
    }
}
