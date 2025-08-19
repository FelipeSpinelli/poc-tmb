namespace Tmb.OrderManagementSystem.Core.Domain;

public interface IOrderRepository
{
    Task InsertAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Order>> SearchOrdersAsync(int skip, int take, CancellationToken cancellationToken);
}