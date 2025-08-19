using Microsoft.EntityFrameworkCore;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Infra.Database;

public class OrderRepository : IOrderRepository
{
    private readonly TmbDbContext _dbContext;

    public async Task InsertAsync(Order order, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        _dbContext.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> SearchOrdersAsync(int skip, int take, CancellationToken cancellationToken)
    {
        var orders = await _dbContext.Orders
            .Skip(skip)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders;
    }
}
