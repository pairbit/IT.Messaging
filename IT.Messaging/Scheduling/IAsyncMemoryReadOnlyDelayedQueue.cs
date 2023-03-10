using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncMemoryReadOnlyDelayedQueue : IAsyncDelayedQueueInformer
{
    Task<long> GetDelayAsync(ReadOnlyMemory<byte> message, string? queue = null);

    Task<long[]> GetDelayAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null);

    Task<MessageDelay[]> GetRangeAsync(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);

    Task<ReadOnlyMemory<byte>[]> GetMessageRangeAsync(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}