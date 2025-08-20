using Tmb.OrderManagementSystem.Core.Application.Ports;

namespace Tmb.OrderManagementSystem.Worker;

public class ServiceBusWorker : BackgroundService
{
    private readonly IMessagingReceiver _busReceiver;

    public ServiceBusWorker(IMessagingReceiver busReceiver)
    {
        _busReceiver = busReceiver;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return _busReceiver.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        return _busReceiver.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
