using Flunt.Notifications;
using Flunt.Validations;

namespace Tmb.OrderManagementSystem.Core.Domain;

public class Order : Notifiable<Notification>
{
    public static class Constraints
    {
        public const int CUSTOMER_MIN_LENGTH = 5;
        public const int CUSTOMER_MAX_LENGTH = 100;

        public const int PRODUCT_MIN_LENGTH = 5;
        public const int PRODUCT_MAX_LENGTH = 100;
        
        public const decimal PRICE_MIN_VALUE = 0.01m;
    }

    public Guid Id { get; init; } = Guid.Empty;
    public string Customer { get; init; } = null!;
    public string Product { get; init; } = null!;
    public decimal Price { get; init; }
    public OrderStatus Status { get; private set; }
    public DateTime CreationDate { get; init; }

    public Order()
    {            
    }

    public Order
    (
        string customer,
        string product,
        decimal price
    )
    {
        AddNotifications(new Contract<Order>()
            .IsLowerThan(customer, Constraints.CUSTOMER_MAX_LENGTH, "Cliente", $"Cliente deve possuir no máximo {Constraints.CUSTOMER_MAX_LENGTH} caracteres!")
            .IsGreaterOrEqualsThan(customer, Constraints.CUSTOMER_MIN_LENGTH, "Cliente", $"Cliente deve possuir ao menos {Constraints.CUSTOMER_MIN_LENGTH} caracteres!")

            .IsLowerThan(product, Constraints.PRODUCT_MAX_LENGTH, "Produto", $"Produto deve possuir no máximo {Constraints.PRODUCT_MAX_LENGTH} caracteres!")
            .IsGreaterOrEqualsThan(product, Constraints.PRODUCT_MIN_LENGTH, "Produto", $"Produto deve possuir ao menos {Constraints.PRODUCT_MIN_LENGTH} caracteres!")

            .IsGreaterThan(price, decimal.Zero, "Valor", $"Valor não pode ser zero!")
            .IsGreaterOrEqualsThan(price, Constraints.PRICE_MIN_VALUE, "Valor", $"Valor deve ser maior ou igual à {Constraints.PRICE_MIN_VALUE:f2}")
        );

        if (!IsValid)
        {
            return;
        }

        Id = Guid.NewGuid();
        Customer = customer;
        Product = product;
        Price = price;
        Status = OrderStatus.Pending;
        CreationDate = DateTime.UtcNow;
    }

    public void ChangeStatus()
    {
        Status = Status switch
        {
            OrderStatus.Pending => OrderStatus.Processing,
            OrderStatus.Processing => OrderStatus.Finished,
            _ => throw new InvalidOperationException($"Order status cannot be changed! [Current: {Status}]")
        };
    }
}