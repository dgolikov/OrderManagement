using OrderManagement.Domain.Common;
using OrderManagement.Domain.Common.Abstractions;
using OrderManagement.Domain.Common.Errors;
using OrderManagement.Domain.Product.Errors;

namespace OrderManagement.Domain.Product;

public sealed class Product : PersistableEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string SKU { get; private set;  }

    private Product(string name, decimal price, string sku)
    {
        Name = name;
        Price = price;
        SKU = sku;
    }

    public static Result<Product> Create(string name, decimal price, string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(new PropertyIsRequiredError(nameof(name)));
        }

        if (string.IsNullOrWhiteSpace(sku))
        {
            return Result.Failure<Product>(new PropertyIsRequiredError(nameof(sku)));
        }

        return new Product(name, price, sku);
    }

    public Result SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Product>(new PropertyIsRequiredError(nameof(name)));
        }

        Name = name;

        return Result.Succsess();
    }

    public Result SetPrice(decimal price)
    {
        if (price < 1 || price > 999999)
        {
            return Result.Failure<Product>(new InvalidPriceValueError());
        }

        Price = price;

        return Result.Succsess();
    }

    public Result SetSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return Result.Failure<Product>(new PropertyIsRequiredError(nameof(sku)));
        }

        SKU = sku;

        return Result.Succsess();
    }
}
