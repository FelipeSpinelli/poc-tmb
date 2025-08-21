using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Ports;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Application.Services;

internal class OrderService : IOrderService, IMessagingHandler<OrderStatusChangingData>
{
    private readonly IOrderRepository _repository;
    private readonly IMessagingSender _busSender;

    public OrderService(IOrderRepository repository, IMessagingSender busSender)
    {
        _repository = repository;
        _busSender = busSender;
    }

    public async Task<Result<Guid>> CreateOrderAsync(OrderCreationData orderCreationData, CancellationToken cancellationToken)
    {
        var order = new Order(
            orderCreationData.Customer,
            orderCreationData.Product,
            orderCreationData.Price);

        var result = new Result<Guid>(order.Id);
        if (!order.IsValid)
        {
            var errors = order.Notifications
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Message));

            foreach (var errorKey in errors)
            {
                result.Errors.Add(errorKey.Key, errorKey.Value);
            }

            return result;
        }

        await _repository.InsertAsync(order, cancellationToken);
        await _busSender.SendAsync(new OrderStatusChangingData(order.Id), cancellationToken);

        return result;
    }

    public async Task HandleAsync(OrderStatusChangingData obj, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderByIdAsync(obj.OrderId, cancellationToken);
        if (order is null)
        {
            return;
        }

        order.ChangeStatus();
        await _repository.UpdateAsync(order, cancellationToken);

        if (order.Status == OrderStatus.Finished)
        {
            return;
        }

        await _busSender.ScheduleAsync(obj, DateTimeOffset.UtcNow.AddSeconds(5), cancellationToken);
    }

    public Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _repository.GetOrderByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<Order>> SearchOrdersAsync(SearchOrdersParameters searchParams, CancellationToken cancellationToken)
    {
        return _repository.SearchOrdersAsync((int)searchParams.Offset, (int)searchParams.PageSize, cancellationToken);
    }
}
