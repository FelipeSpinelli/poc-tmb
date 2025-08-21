using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Tmb.OrderManagementSystem.Core.Application.Models;
using Tmb.OrderManagementSystem.Core.Application.Ports;

namespace Tmb.OrderManagementSystem.Core.Infra.Messaging;
internal class AzureServiceBusAdapter : IMessagingSender, IMessagingReceiver
{
    private readonly ServiceBusSender? _serviceBusSender;
    private readonly ServiceBusProcessor? _serviceBusProcessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AzureServiceBusAdapter> _logger;

    public AzureServiceBusAdapter(
        IServiceProvider serviceProvider,
        ServiceBusClient serviceBusClient,
        MessagingSettings settings,
        ILogger<AzureServiceBusAdapter> logger)
    {
        _serviceProvider = serviceProvider;
        _serviceBusSender = settings.EnableSent ? serviceBusClient.CreateSender(settings.QueueName) : default;
        _logger = logger;

        if (!settings.EnableReceive)
        {
            return;
        }
        
        _serviceBusProcessor = serviceBusClient.CreateProcessor(settings.QueueName);
        _serviceBusProcessor!.ProcessMessageAsync += MessageHandler;
        _serviceBusProcessor!.ProcessErrorAsync += ErrorHandler;
    }

    public async Task SendAsync<TSentMessage>(TSentMessage obj, CancellationToken cancellationToken)
        where TSentMessage : new()
    {
        var message = CreateMessage(obj);

        await _serviceBusSender!.SendMessageAsync(message);

        _logger.LogInformation("Message sent to Azure Service Bus!");
    }

    public async Task ScheduleAsync<TSentMessage>(TSentMessage obj, DateTimeOffset offset, CancellationToken cancellationToken)
        where TSentMessage : new()
    {
        var message = CreateMessage(obj);
        message.ScheduledEnqueueTime = offset;

        await _serviceBusSender!.ScheduleMessageAsync(message, message.ScheduledEnqueueTime, cancellationToken);

        _logger.LogInformation("Message scheduled to Azure Service Bus!");
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
            _logger.LogInformation("Message received from Azure Service Bus!");
            var body = args.Message.Body.ToObjectFromJson<AzureServiceBusMessageBody>();

            await TryHandleMessageAsync(body!);

            await args.CompleteMessageAsync(args.Message);
            
            _logger.LogInformation("Message succesfully handled!");
        }
        catch (Exception ex)
        {
        }
    }

    private async Task TryHandleMessageAsync(AzureServiceBusMessageBody body)
    {
        var messageType = typeof(Result).Assembly.GetType(body.Type)!;
        var handlerType = typeof(IMessagingHandler<>).MakeGenericType(messageType)!;

        using var scope = _serviceProvider.CreateScope();
        var handler = scope!.ServiceProvider.GetService(handlerType);

        if (handler is null)
        {
            return;
        }

        var message = JsonSerializer.Deserialize(body.Payload, messageType)!;
        var handleMethod = handlerType.GetMethod("HandleAsync");
        var task = (Task)handleMethod!.Invoke(handler, new object[] { message, CancellationToken.None })!;

        await task;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        return Task.CompletedTask;
    }
}
