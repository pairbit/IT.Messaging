using System;
using System.Threading.Tasks;
using IT.Messaging.Scheduling;

namespace IT.Messaging.Redis;

public class SortedSetDelayedPublisher : IDelayedPublisher
{
    public bool DelayPublish<T>(long delay, T message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish<T>(long delay, T[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public long DelayPublish<T>(MessageDelay<T>[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish(long delay, ReadOnlyMemory<byte> message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish(long delay, ReadOnlyMemory<byte>[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public long DelayPublish(MessageDelay[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync<T>(long delay, T message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync<T>(long delay, T[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DelayPublishAsync<T>(MessageDelay<T>[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync(long delay, ReadOnlyMemory<byte> message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync(long delay, ReadOnlyMemory<byte>[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DelayPublishAsync(MessageDelay[] messages, string? channel = null)
    {
        throw new NotImplementedException();
    }
}