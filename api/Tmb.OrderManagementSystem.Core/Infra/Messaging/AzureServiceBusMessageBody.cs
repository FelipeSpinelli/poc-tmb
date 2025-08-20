using System.Text.Json;

namespace Tmb.OrderManagementSystem.Core.Infra.Messaging;

public class AzureServiceBusMessageBody
{
    public string Type { get; init; } = null!;
    public string Payload { get; init; } = null!;

    public static AzureServiceBusMessageBody CreateFrom<T>(T obj)
        where T : new()
    {
        return new()
        {
            Type = typeof(T).FullName ?? typeof(T).Name,
            Payload = JsonSerializer.Serialize(obj)
        };
    }
}
