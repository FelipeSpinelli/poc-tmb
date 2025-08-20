using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tmb.OrderManagementSystem.Core.Application.Ports;

namespace Tmb.OrderManagementSystem.Core.Infra.Messaging;

public static class MessagingConfigurationExtensions
{
    public static IServiceCollection ConfigureMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(MessagingSettings.SECTION_NAME).Get<MessagingSettings>()!;

        return services
            .AddSingleton(settings)
            .AddSingleton(new ServiceBusClient(configuration.GetConnectionString("TmbOrderManagementSystemASB")))
            .AddScoped<IMessagingSender, AzureServiceBusAdapter>()
            .AddSingleton<IMessagingReceiver, AzureServiceBusAdapter>();
    }
}