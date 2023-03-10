using System;
using System.Collections.Generic;

namespace IT.Messaging.Scheduling;

public interface IMemoryReadOnlyDelayedQueue : IAsyncMemoryReadOnlyDelayedQueue, IDelayedQueueInformer
{
    long GetDelay(ReadOnlyMemory<byte> message, string? queue = null);

    long[] GetDelay(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null);

    MessageDelay[] GetRange(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);

    ReadOnlyMemory<byte>[] GetMessageRange(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}