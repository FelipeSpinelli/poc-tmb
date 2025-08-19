using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmb.OrderManagementSystem.Core.Application.Services;
using Tmb.OrderManagementSystem.Core.Application.Services.Abstractions;
using Tmb.OrderManagementSystem.Core.Infra.Database;

namespace Tmb.OrderManagementSystem.Core.Application.Configuration;

public static class ApplicationExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<IOrderService, OrderService>();

        services.AddDbContext<TmbDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("TmbOrderManagementSystemDb"));            
        });

        services.AddSingleton(new ServiceBusClient(configuration.GetConnectionString("TmbOrderManagementSystemASB")));

        return services;
    }
}
