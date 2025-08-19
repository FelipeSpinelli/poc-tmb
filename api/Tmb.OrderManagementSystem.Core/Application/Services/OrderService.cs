using Microsoft.EntityFrameworkCore;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;
using Tmb.OrderManagementSystem.Core.Domain;
using Tmb.OrderManagementSystem.Core.Infra.Database;

namespace Tmb.OrderManagementSystem.Core.Application.Services;

internal class OrderService : IOrderService
{
    private readonly TmbDbContext _dbContext;

    public OrderService(TmbDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> CreateOrderAsync(OrderCreationData orderCreationData, CancellationToken cancellationToken)
    {
        var order = new Order(orderCreationData.Customer, orderCreationData.Product, orderCreationData.Price);
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

        await _dbContext.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return result;
    }

    public Task<Order?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> SearchOrdersAsync(SearchOrdersParameters searchParams, CancellationToken cancellationToken)
    {
        var orders =  await _dbContext.Orders
            .Skip((int)searchParams.Offset)
            .Take((int)searchParams.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return orders;
    }
}
