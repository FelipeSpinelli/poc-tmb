namespace Tmb.OrderManagementSystem.Api.Endpoints;

public static class EndpointsExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.AddOrderCreation();
    }
}
