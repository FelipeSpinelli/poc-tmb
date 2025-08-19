using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;

public interface IOrderService
{
    Task<Result<Guid>> CreateOrderAsync(OrderCreationData orderCreationData, CancellationToken cancellationToken);
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Order>> SearchOrdersAsync(SearchOrdersParameters searchParams, CancellationToken cancellationToken);
}
