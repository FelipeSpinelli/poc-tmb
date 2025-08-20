using Tmb.OrderManagementSystem.Worker;
using Tmb.OrderManagementSystem.Core.Application.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ServiceBusWorker>();

builder.Services.ConfigureApplication(builder.Configuration);

var host = builder.Build();
host.Run();
