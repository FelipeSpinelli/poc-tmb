namespace Tmb.OrderManagementSystem.Core.Application.Ports;

public interface IMessagingHandler<T> where T : new()
{
    Task HandleAsync(T obj, CancellationToken cancellationToken);
}
