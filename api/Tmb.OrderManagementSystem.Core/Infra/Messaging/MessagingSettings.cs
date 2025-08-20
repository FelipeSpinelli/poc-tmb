namespace Tmb.OrderManagementSystem.Core.Infra.Messaging;

public record MessagingSettings
{
    public const string SECTION_NAME = nameof(MessagingSettings);

    public bool EnableSent { get; init; }
    public bool EnableReceive { get; init; }
    public string ConnectionStringName { get; init; } = null!;
    public string QueueName { get; init; } = null!;
}