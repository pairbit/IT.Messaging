using System;

namespace IT.Messaging.Scheduling;

public interface IMemoryDelayedQueue : 
    IAsyncMemoryDelayedQueue, 
    IMemoryReadOnlyDelayedQueue,
    IMemoryDelayedPublisher,
    IDelayedQueueCleaner
{
    bool Delete(ReadOnlyMemory<byte> message, string? queue = null);
}