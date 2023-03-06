using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class MemoryQueueSubscriber : IMemorySubscriber
{
    private readonly IDatabase _db;

    public MemoryQueueSubscriber(IDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? key = null)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>[], string?> handler, string? key = null)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? key = null)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? key = null)
    {
        throw new NotImplementedException();
    }

    public void Unsubscribe(string key)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeAll()
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(string key)
    {
        throw new NotImplementedException();
    }
}