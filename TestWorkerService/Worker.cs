using IT.Messaging;

namespace TestWorkerService;

public class Worker : IHostedService
{
    private readonly ILogger<Worker> _logger;
    private readonly IChannel _channel;

    public Worker(ILogger<Worker> logger, IChannel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _channel.SubscribeAsync<Guid>(ProcessAsync, key: "q1");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _channel.UnsubscribeAllAsync();
    }

    private async Task ProcessAsync(Guid value, string? queue, CancellationToken token)
    {
        Console.WriteLine($"Processing Queue '{queue}' processed with '{value}'");

        await Task.Delay(TimeSpan.FromSeconds(10), token);

        Console.WriteLine($"Queue '{queue}' processed with '{value}'");

        await _channel.PublishAsync(value, "q2");
    }
}