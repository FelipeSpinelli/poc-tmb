using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Ports;
using Tmb.OrderManagementSystem.Core.Application.Services;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;
using Tmb.OrderManagementSystem.Core.Infra.Database;
using Tmb.OrderManagementSystem.Core.Infra.Messaging;

namespace Tmb.OrderManagementSystem.Core.Application.Configuration;

public static class ApplicationExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .ConfigureDatabase(configuration)
            .ConfigureMessaging(configuration)
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<IMessagingHandler<OrderStatusChangingData>, OrderService>();
    }
}
