using Microsoft.AspNetCore.Mvc;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;

namespace Tmb.OrderManagementSystem.Api.Endpoints;

public static class OrderCreation
{
    public class Request
    {
        public string Customer { get; init; } = null!;
        public string Product { get; init; } = null!;
        public decimal Price { get; init; }

        public OrderCreationData AsOrderCreationData() => new(Customer, Product, Price);
    }

    internal static void AddOrderCreation(this WebApplication app)
    {
        app.MapPost("/orders", async ([FromServices] IOrderService orderService, [FromBody] Request request, CancellationToken cancellationToken) =>
        {
            var orderCreationData = request.AsOrderCreationData();
            var orderCreationResult = await orderService.CreateOrderAsync(orderCreationData, cancellationToken);

            if (orderCreationResult.IsValid)
            {
                var id = orderCreationResult.Data!;
                return (IResult)TypedResults.Created($"/orders/{id}", new { Id = id });
            }

            return (IResult)TypedResults.BadRequest(orderCreationResult.Errors);
        })
        .WithName("CreateOrder")
        .WithOpenApi();
    }
}
