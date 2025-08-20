using Microsoft.AspNetCore.Mvc;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;

namespace Tmb.OrderManagementSystem.Api.Endpoints;

public static class OrderCreation
{
    public class Request
    {
        public string Cliente { get; init; } = null!;
        public string Produto { get; init; } = null!;
        public decimal Valor { get; init; }

        public OrderCreationData AsOrderCreationData() => new(Cliente, Produto, Valor);
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