using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncMemoryDelayedQueue : 
    IAsyncMemoryReadOnlyDelayedQueue, 
    IAsyncMemoryDelayedPublisher,
    IAsyncDelayedQueueCleaner
{
    Task<bool> DeleteAsync(ReadOnlyMemory<byte> message, string? queue = null);

    Task<long> DeleteAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? queue = null);
}