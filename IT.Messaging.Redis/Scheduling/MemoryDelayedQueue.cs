using IT.Messaging.Scheduling;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Redis.Scheduling;

public class MemoryDelayedQueue : IMemoryDelayedQueue
{
    protected readonly IDatabase _db;

    public MemoryDelayedQueue(IDatabase db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public bool Clean(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long CleanAll(IEnumerable<string>? queues = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> CleanAllAsync(IEnumerable<string>? queues = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CleanAsync(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long CleanRange(long minDelay, long maxDelay, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> CleanRangeAsync(long minDelay, long maxDelay, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish(long delay, ReadOnlyMemory<byte> message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool DelayPublish(long delay, IEnumerable<ReadOnlyMemory<byte>> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public long DelayPublish(IEnumerable<MessageDelay> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync(long delay, ReadOnlyMemory<byte> message, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DelayPublishAsync(long delay, IEnumerable<ReadOnlyMemory<byte>> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DelayPublishAsync(IEnumerable<MessageDelay> messages, string? channel = null)
    {
        throw new NotImplementedException();
    }

    public bool Delete(ReadOnlyMemory<byte> message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long Delete(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(ReadOnlyMemory<byte> message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> DeleteAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long ExistsAll(IEnumerable<string>? queues = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> ExistsAllAsync(IEnumerable<string>? queues = null)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long GetDelay(ReadOnlyMemory<byte> message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long[] GetDelay(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetDelayAsync(ReadOnlyMemory<byte> message, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long[]> GetDelayAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long GetLength(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetLengthAsync(string? queue = null)
    {
        throw new NotImplementedException();
    }

    public long GetLengthRange(long minDelay, long maxDelay, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetLengthRangeAsync(long minDelay, long maxDelay, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public ReadOnlyMemory<byte>[] GetMessageRange(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<ReadOnlyMemory<byte>[]> GetMessageRangeAsync(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public MessageDelay[] GetRange(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }

    public Task<MessageDelay[]> GetRangeAsync(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null)
    {
        throw new NotImplementedException();
    }
}
