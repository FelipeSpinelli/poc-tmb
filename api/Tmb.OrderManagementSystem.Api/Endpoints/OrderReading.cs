using Microsoft.AspNetCore.Mvc;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Api.Endpoints;

public static class OrderReading
{
    public class PaginationRequest
    {
        [FromQuery(Name = "pageNumber")]
        public int? PageNumber { get; set; }

        [FromQuery(Name = "pageSize")]
        public int? PageSize { get; set; }

        public uint GetPageNumber()
        {
            if (!PageNumber.HasValue || PageNumber.Value < 1)
            {
                return 1;
            }

            return (uint)PageNumber.Value;
        }

        public uint GetPageSize()
        {
            if (!PageSize.HasValue || PageSize.Value < 1)
            {
                return 10;
            }

            return (uint)PageSize.Value;
        }
    }

    public class Response
    {
        public string Id { get; init; } = null!;
        public string Cliente { get; init; } = null!;
        public string Produto { get; init; } = null!;
        public decimal Valor { get; init; }
        public string Status { get; init; } = null!;
        public string DataCriacao { get; init; } = null!;

        public static Response FromOrder(Order order) => new()
        {
            Id = order.Id.ToString(),
            Cliente = order.Customer,
            Produto = order.Product,
            Valor = order.Price,
            DataCriacao = order.CreationDate.ToString("yyyy-MM-dd"),
            Status = order.Status switch
            {
                OrderStatus.Pending => "Pendente",
                OrderStatus.Processing => "Processando",
                OrderStatus.Finished => "Finalizado",
                _ => "-"
            }
        };
    }

    internal static void AddGetOrderById(this WebApplication app)
    {
        app.MapGet("/orders/{id}", async ([FromServices] IOrderService orderService, [FromRoute] string id, CancellationToken cancellationToken) =>
        {
            var order = await orderService.GetOrderByIdAsync(Guid.Parse(id), cancellationToken);
            if (order is null)
            {
                return (IResult)TypedResults.NotFound();
            }
            
            var response = Response.FromOrder(order);

            return (IResult)TypedResults.Ok(response);
        })
        .WithName("GetOrderById")
        .WithOpenApi();
    }

    internal static void AddListOrders(this WebApplication app)
    {
        app.MapGet("/orders", async ([FromServices] IOrderService orderService, [AsParameters] PaginationRequest pagination, CancellationToken cancellationToken) =>
        {
            var parameters = new SearchOrdersParameters
            {
                PageNumber = pagination.GetPageNumber(),
                PageSize = pagination.GetPageSize()
            };
            var orders = await orderService.SearchOrdersAsync(parameters, cancellationToken);
            var response = orders.Select(x => Response.FromOrder(x));

            return (IResult)TypedResults.Ok(response);
        })
        .WithName("ListOrders")
        .WithOpenApi();
    }
}
