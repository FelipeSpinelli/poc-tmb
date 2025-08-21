using FluentAssertions;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Tests.Core.Domain;

public class OrderTests
{
    private const string VALID_CUSTOMER = "Valid Customer";
    private const string VALID_PRODUCT = "Valid Product";
    private const decimal VALID_PRICE = 150.00m;

    [Fact]
    public void Create_WithValidArgs_ShouldBeValid()
    {
        //Arrange

        //Act
        var order = new Order(VALID_CUSTOMER, VALID_PRODUCT, VALID_PRICE);

        //Assert
        order.IsValid.Should().BeTrue();
        order.Customer.Should().Be(VALID_CUSTOMER);
        order.Product.Should().Be(VALID_PRODUCT);
        order.Price.Should().Be(VALID_PRICE);
        order.Notifications.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(GetInvalidParamsToOrderCreation))]
    public void Create_WithInvalidArgs_ShouldBeInvalid(string customer, string product, decimal price, string invalidKey)
    {
        //Arrange

        //Act
        var order = new Order(customer, product, price);

        //Assert
        order.IsValid.Should().BeFalse();
        order.Notifications.Should().NotBeEmpty();
        order.Notifications.Any(x => x.Key == invalidKey).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(GetValidOrderToStatusChanging))]
    public void ChangeStatus_WithValidStatus_ShouldBeAsExpected(Order order, OrderStatus expectedStatus)
    {
        //Arrange

        //Act
        order.ChangeStatus();        

        //Assert
        order.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public void ChangeStatus_AfterFinished_ShouldThrow()
    {
        //Arrange
        var order = new Order(VALID_CUSTOMER, VALID_PRODUCT, VALID_PRICE);

        //Act
        order.ChangeStatus();
        order.ChangeStatus();
        var action = () => order.ChangeStatus();

        //Assert
        action.Should().Throw<InvalidOperationException>();
    }

    public static IEnumerable<object[]> GetInvalidParamsToOrderCreation()
    {
        yield return new object[] { "", VALID_PRODUCT, VALID_PRICE, "Cliente" };
        yield return new object[] { string.Join("", Enumerable.Range(0, Order.Constraints.CUSTOMER_MIN_LENGTH - 1).Select(x => "X")), VALID_PRODUCT, VALID_PRICE, "Cliente" };
        yield return new object[] { string.Join("", Enumerable.Range(1, Order.Constraints.CUSTOMER_MAX_LENGTH + 1).Select(x => "X")), VALID_PRODUCT, VALID_PRICE, "Cliente" };

        yield return new object[] { VALID_CUSTOMER, "", VALID_PRICE, "Produto" };
        yield return new object[] { VALID_CUSTOMER, string.Join("", Enumerable.Range(0, Order.Constraints.CUSTOMER_MIN_LENGTH - 1).Select(x => "X")), VALID_PRICE, "Produto" };
        yield return new object[] { VALID_CUSTOMER, string.Join("", Enumerable.Range(1, Order.Constraints.CUSTOMER_MAX_LENGTH + 1).Select(x => "X")), VALID_PRICE, "Produto" };

        yield return new object[] { VALID_CUSTOMER, VALID_PRODUCT, 0.00m, "Valor" };
        yield return new object[] { VALID_CUSTOMER, VALID_PRODUCT, Order.Constraints.PRICE_MIN_VALUE - 0.001m, "Valor" };
    }

    public static IEnumerable<object[]> GetValidOrderToStatusChanging()
    {
        var order1 = new Order(VALID_CUSTOMER, VALID_PRODUCT, VALID_PRICE);
        var order2 = new Order(VALID_CUSTOMER, VALID_PRODUCT, VALID_PRICE);
        yield return new object[] { order1, OrderStatus.Processing };

        order2.ChangeStatus();
        yield return new object[] { order2, OrderStatus.Finished };
    }
}