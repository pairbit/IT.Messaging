using IT.Messaging;
using TestWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //services.AddSingleton<IChannel>(new IT.Messaging.Redis.Queue())
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
