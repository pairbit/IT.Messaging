using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class NotImplementedMemorySubscriber : IMemorySubscriber
{
    public static readonly IMemorySubscriber Default = new NotImplementedMemorySubscriber();

    private NotImplementedMemorySubscriber()
    {

    }

    public void Subscribe(MemoryHandler? handler, MemoryBatchHandler? batchHandler = null, string? key = null)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? key = null)
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