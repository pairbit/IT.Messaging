using System;

namespace IT.Messaging.Scheduling;

public interface IMemoryReadOnlyDelayedQueue : IAsyncMemoryReadOnlyDelayedQueue, IDelayedQueueInformer
{
    long GetDelay(ReadOnlyMemory<byte> message, string? queue = null);

    MessageDelay[] GetRange(long? minDelay, long? maxDelay, bool withDelay = false, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}