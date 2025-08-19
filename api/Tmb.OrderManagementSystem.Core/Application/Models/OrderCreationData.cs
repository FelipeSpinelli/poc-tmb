namespace Tmb.OrderManagementSystem.Core.Application.Models;

public record OrderCreationData
(
    string Customer,
    string Product,
    decimal Price
);