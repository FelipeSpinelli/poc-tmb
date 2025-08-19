namespace Tmb.OrderManagementSystem.Core.Application.Models;

public record SearchOrdersParameters
{
    public uint PageSize { get; init; }
    public uint PageNumber { get; init; }

    public uint Offset => (PageNumber - 1) * PageSize;
}