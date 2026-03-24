using Bogus;
using MongoDB.Driver;
using OrderManagement.Domain.Product;

namespace OrderManagement.DataSeeder;

static class Program
{
    private const int ProductCount = 1000;
    private const string ConnectionString = "mongodb://ordermnguser:12345678Aa@localhost:29700/OrderManagement?authSource=admin";
    private const string DatabaseName = "OrderManagement";
    private const string CollectionName = "products";

    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("=== Product Test Data Seeder ===");
        Console.WriteLine($"Generating {ProductCount} products...");

        try
        {
            var imageUrls = await ReadImageUrlsAsync();
            Console.WriteLine($"Loaded {imageUrls.Count} image URLs from links.txt");

            var productFaker = CreateProductFaker(imageUrls);

            var products = productFaker.Generate(ProductCount);
            Console.WriteLine($"Generated {products.Count} products");

            var collection = GetMongoCollection();
            Console.WriteLine("Connected to MongoDB");

            var existingCount = await collection.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            if (existingCount > 0)
            {
                Console.WriteLine($"Found {existingCount} existing products.");
                Console.Write("Delete existing products? (y/N): ");
                var response = Console.ReadLine();
                if (response?.ToLower() == "y")
                {
                    await collection.DeleteManyAsync(FilterDefinition<Product>.Empty);
                    Console.WriteLine("Deleted existing products.");
                }
            }

            await collection.InsertManyAsync(products);
            Console.WriteLine($"Successfully inserted {products.Count} products!");

            var finalCount = await collection.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            Console.WriteLine($"Total products in database: {finalCount}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1;
        }
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

    private static IMongoCollection<Product> GetMongoCollection()
    {
        var client = new MongoClient(ConnectionString);
        var database = client.GetDatabase(DatabaseName);
        return database.GetCollection<Product>(CollectionName);
    }
}
