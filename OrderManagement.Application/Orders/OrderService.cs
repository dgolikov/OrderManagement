using OrderManagement.Domain.Common;
using OrderManagement.Domain.Order;
using OrderManagement.Domain.Product;

namespace OrderManagement.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderNumberGenerator _orderNumberGenerator;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderNumberGenerator orderNumberGenerator,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderNumberGenerator = orderNumberGenerator;
        _productRepository = productRepository;
    }

    public async Task<Result<OrderView>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var orderResult = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (orderResult.IsFailure)
        {
            return Result.Failure<OrderView>(orderResult.Error);
        }

        var products = await LoadProductsForOrder(orderResult.Value, cancellationToken);
        var orderView = OrderView.Create(orderResult.Value, products);

        return orderView;
    }

    public async Task<Page<OrderView>> GetPageAsync(QueryParams queryParams, CancellationToken cancellationToken)
    {
        var ordersPage = await _orderRepository.GetPageAsync(queryParams, cancellationToken);
        var products = await LoadProductsForOrders(ordersPage.Items, cancellationToken);

        var orderViews = OrderView.CreateCollection(ordersPage.Items, products);

        return new Page<OrderView>(
            ordersPage.PageNumber,
            ordersPage.PageSize,
            ordersPage.Total,
            orderViews);
    }

    private async Task<IReadOnlyDictionary<Guid, Product>> LoadProductsForOrders(
        IEnumerable<Order> orders,
        CancellationToken cancellationToken)
    {
        var productIds = orders
            .SelectMany(o => o.LineItems)
            .Select(li => li.ProductId)
            .Distinct();

        return await _productRepository.GetByIdsAsync(productIds, cancellationToken);
    }

    private async Task<IReadOnlyDictionary<Guid, Product>> LoadProductsForOrder(
        Order order,
        CancellationToken cancellationToken)
    {
        var productIds = order.LineItems.Select(li => li.ProductId);
        return await _productRepository.GetByIdsAsync(productIds, cancellationToken);
    }

    public async Task<Result<Order>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var lineItemRequests = request.LineItems.ToList();
        var productIds = lineItemRequests.Select(li => li.ProductId).Distinct();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var lineItems = lineItemRequests.Select(li =>
        {
            var product = products[li.ProductId];
            return new OrderLineItem(li.ProductId, li.Quantity, product.Price);
        });

        var result = Order.Create(request.UserId, lineItems);

        if (result.IsFailure)
        {
            return result;
        }

        var orderNumber = await _orderNumberGenerator.GenerateNextOrderNumberAsync(cancellationToken);
        result.Value.SetOrderNumber(orderNumber);

        return await _orderRepository.CreateAsync(result.Value, cancellationToken);
    }

    public async Task<Result<Order>> UpdateAsync(Guid id, CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return result;
        }

        var order = result.Value;

        var lineItemRequests = request.LineItems.ToList();
        var productIds = lineItemRequests.Select(li => li.ProductId).Distinct();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var lineItems = lineItemRequests.Select(li =>
        {
            var product = products[li.ProductId];
            return new OrderLineItem(li.ProductId, li.Quantity, product.Price);
        });

        var updateResult = order.UpdateLineItems(lineItems);

        if (updateResult.IsFailure)
        {
            return Result.Failure<Order>(updateResult.Error);
        }

        order.SetStatus(OrderStatus.Approved);

        await _orderRepository.UpdateAsync(order, cancellationToken);
        return order;
    }

    public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return _orderRepository.DeleteAsync(id, cancellationToken);
    }
}
