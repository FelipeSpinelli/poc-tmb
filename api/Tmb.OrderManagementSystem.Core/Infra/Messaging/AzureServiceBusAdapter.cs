using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Tmb.OrderManagementSystem.Core.Application.Ports;

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
internal class AzureServiceBusAdapter : IMessagingSender, IMessagingReceiver
{
    private readonly ServiceBusSender? _serviceBusSender;
    private readonly ServiceBusProcessor? _serviceBusProcessor;
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusAdapter(
        IServiceProvider serviceProvider,
        ServiceBusClient serviceBusClient,
        MessagingSettings settings)
    {
        _serviceProvider = serviceProvider;
        _serviceBusSender = settings.EnableSent ? serviceBusClient.CreateSender(settings.QueueName) : default;
        _serviceBusProcessor = settings.EnableReceive ? serviceBusClient.CreateProcessor(settings.QueueName) : default;

        if (!settings.EnableReceive)
        {
            return;
        }

        _serviceBusProcessor!.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor!.ProcessErrorAsync += ErrorHandler;
    }

    public async Task SendAsync<TSentMessage>(TSentMessage obj, CancellationToken cancellationToken)
        where TSentMessage : new()
    {
        var message = CreateMessage(obj);
        
        await _serviceBusSender!.SendMessageAsync(message);
    }

    public async Task ScheduleAsync<TSentMessage>(TSentMessage obj, DateTimeOffset offset, CancellationToken cancellationToken)
        where TSentMessage : new()
    {
        var message = CreateMessage(obj);
        message.ScheduledEnqueueTime = offset;

        await _serviceBusSender!.ScheduleMessageAsync(message, message.ScheduledEnqueueTime, cancellationToken);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _serviceBusProcessor!.StartProcessingAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _serviceBusProcessor!.StopProcessingAsync(cancellationToken);
    }

    private static ServiceBusMessage CreateMessage<TSentMessage>(TSentMessage obj) where TSentMessage : new()
    {
        var body = AzureServiceBusMessageBody.CreateFrom(obj);
        var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(body));
        return message;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToObjectFromJson<AzureServiceBusMessageBody>();

            await TryHandleMessageAsync(body);

            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
        }
    }

    private async Task TryHandleMessageAsync(AzureServiceBusMessageBody body)
    {
        using var scope = _serviceProvider.CreateScope();
        using var handler = scope!.Get
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        return Task.CompletedTask;
    }
}
