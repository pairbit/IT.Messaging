using System;

namespace IT.Messaging;

public interface IMemoryReadOnlyQueue : IAsyncMemoryReadOnlyQueue, IQueueInformer
{
    long GetPosition(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null);

    ReadOnlyMemory<byte> GetByIndex(long index, string? queue = null);

    ReadOnlyMemory<byte>[] GetRange(long start = 0, long stop = -1, string? queue = null);
}