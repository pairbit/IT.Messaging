using System;
using System.Collections.Generic;

namespace IT.Messaging;

public interface IMemoryQueue : IAsyncMemoryQueue, IMemoryReadOnlyQueue, IMemoryChannel, IQueueTrimmer, IQueueMover, IQueueCleaner
{
    //Rename
    //Move
    //MoveAll

    long MoveAll(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null);

    long Delete(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);

    long Delete(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null);

    //DeleteRange
}