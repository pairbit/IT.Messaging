using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryQueue : IAsyncMemoryReadOnlyQueue, IAsyncQueueTrimmer, IAsyncQueueCleaner, IAsyncMemoryChannel
{
    Task<long> DeleteAsync(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);

    Task<long> DeleteAsync(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null);
}