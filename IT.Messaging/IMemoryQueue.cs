using System;
using System.Collections.Generic;

namespace IT.Messaging;

public interface IMemoryQueue : IAsyncMemoryQueue, IMemoryReadOnlyQueue, IMemoryChannel, IQueueTrimmer, IQueueCleaner
{
    //Rename
    //Move
    //MoveAll

    long Delete(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);

    long Delete(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null);

    //DeleteRange
}