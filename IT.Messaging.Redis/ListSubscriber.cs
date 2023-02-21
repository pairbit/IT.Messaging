using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class ListSubscriber : ISubscriber
{
    private readonly IDatabase _db;
    private readonly IRedisValueDeserializer _deserializer;

    public ListSubscriber(
        IDatabase db,
        IRedisValueDeserializer deserializer)
    {
        _db = db;
        _deserializer = deserializer;
    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public async Task SubscribeAsync<T>(Action<T, string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public async Task SubscribeAsync<T>(Action<T[], string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T>(Action<T, string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public void Subscribe<T>(Action<T[], string?> handler, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync() => throw new NotImplementedException();

    public Task UnsubscribeAsync(string channel) => throw new NotImplementedException();

    public void UnsubscribeAll() => throw new NotImplementedException();

    public void Unsubscribe(string channel) => throw new NotImplementedException();
}