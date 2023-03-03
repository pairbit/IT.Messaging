using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryQueue : IAsyncMemoryReadOnlyQueue, IAsyncQueueCleaner, IAsyncMemoryChannel
{
    Task<long> DeleteAsync(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);
}