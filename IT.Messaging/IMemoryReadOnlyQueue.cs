using System;

namespace IT.Messaging;

public interface IMemoryReadOnlyQueue : IAsyncMemoryReadOnlyQueue, IQueueInformer
{
    long GetPosition(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null);
}