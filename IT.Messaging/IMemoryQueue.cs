using System;

namespace IT.Messaging;

public interface IMemoryQueue : IAsyncMemoryQueue, IMemoryReadOnlyQueue, IMemoryChannel, IQueueCleaner
{
    //Rename
    //Move
    //MoveAll

    long Delete(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);

    //DeleteRange
}