namespace Tmb.OrderManagementSystem.Core.Application.Ports;

public interface IMessagingSender
{
    Task SendAsync<T>(T obj, CancellationToken cancellationToken) where T : new ();
    Task ScheduleAsync<T>(T obj, DateTimeOffset offset, CancellationToken cancellationToken) where T : new();
}