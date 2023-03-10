using IT.Messaging.Scheduling;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Redis.Scheduling;

public class DelayedQueue : MemoryDelayedQueue, IDelayedQueue
{
    public DelayedQueue(IDatabase db) : base(db)
    {
    }

    public bool DelayPublish<T>(long delay, T message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish<T>(long delay, IEnumerable<T> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public long DelayPublish<T>(IEnumerable<MessageDelay<T>> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync<T>(long delay, T message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync<T>(long delay, IEnumerable<T> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DelayPublishAsync<T>(IEnumerable<MessageDelay<T>> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool Delete<T>(T message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long Delete<T>(IEnumerable<T> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync<T>(T message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DeleteAsync<T>(IEnumerable<T> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long GetDelay<T>(T message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long[] GetDelay<T>(IEnumerable<T> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetDelayAsync<T>(T message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long[]> GetDelayAsync<T>(IEnumerable<T> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public T[] GetMessageRange<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<T[]> GetMessageRangeAsync<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public MessageDelay<T>[] GetRange<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDelay<T>[]> GetRangeAsync<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }
}