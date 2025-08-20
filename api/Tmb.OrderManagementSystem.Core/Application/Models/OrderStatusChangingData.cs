namespace Tmb.OrderManagementSystem.Core.Application.Models;

public record OrderStatusChangingData
{
    public Guid OrderId { get; init; }

    public OrderStatusChangingData()
    {            
    }

    public OrderStatusChangingData(Guid orderId)
    {
        OrderId = orderId;
    }
}