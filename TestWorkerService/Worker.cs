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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _channel.SubscribeAsync<Guid>(ProcessAsync, key: "q1");

        await _channel.SubscribeAsync<Guid>(ProcessAsync, ProcessAllAsync, key: "q1-multi");

        await _channel.SubscribeAsync<Guid>(null, RollbackAllAsync, key: "q1-rollback");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _channel.UnsubscribeAllAsync();
    }

    private async Task<bool> ProcessAsync(Guid value, string? queue, CancellationToken token)
    {
        Console.WriteLine($"Processing '{value}' from queue '{queue}'");

        await Task.Delay(TimeSpan.FromSeconds(10), token);

        Console.WriteLine($"Processed '{value}' from queue '{queue}'");

        await _channel.PublishAsync(value, "q1-res");

        return true;
    }

    private async Task<bool[]> ProcessAllAsync(IReadOnlyList<Guid> values, string? queue, CancellationToken token)
    {
        for (int i = 0; i < values.Count; i++)
        {
            Console.WriteLine($"Processing '{values[i]}' from queue '{queue}'");
        }

        await Task.Delay(TimeSpan.FromSeconds(10), token);

        for (int i = 0; i < values.Count; i++)
        {
            Console.WriteLine($"Processed '{values[i]}' from queue '{queue}'");
        }

        await _channel.PublishAsync(values, "q1-multi-res");

        return Batch.True;
    }

    private async Task<bool[]> RollbackAllAsync(IReadOnlyList<Guid> values, string? queue, CancellationToken token)
    {
        for (int i = 0; i < values.Count; i++)
        {
            Console.WriteLine($"Processing '{values[i]}' from queue '{queue}'");
        }

        await Task.Delay(TimeSpan.FromSeconds(10), token);

        return Batch.False;
    }
}