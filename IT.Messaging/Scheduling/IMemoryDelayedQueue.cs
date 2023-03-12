using System;
using System.Collections.Generic;

namespace IT.Messaging.Scheduling;

public interface IMemoryDelayedQueue : 
    IAsyncMemoryDelayedQueue, 
    IMemoryReadOnlyDelayedQueue,
    IMemoryDelayedPublisher,
    IDelayedQueueCleaner,
    IQueueMover
{
    bool Delete(ReadOnlyMemory<byte> message, string? queue = null);

    long Delete(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null);
}