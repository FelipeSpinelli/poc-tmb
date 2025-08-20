using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Infra.Database;

public static class DatabaseConfigurationExtensions
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddDbContext<TmbDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("TmbOrderManagementSystemDb"));
            });
    }
}
