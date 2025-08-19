namespace Tmb.OrderManagementSystem.Core.Application.Models;

public record OrderCreationData
{
    public string Customer { get; init; } = null!;
    public string Product { get; init; } = null!;
    public decimal Price { get; init; }
}