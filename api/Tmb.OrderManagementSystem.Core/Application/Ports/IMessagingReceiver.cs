namespace Tmb.OrderManagementSystem.Core.Application.Ports;

public interface IMessagingReceiver
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}