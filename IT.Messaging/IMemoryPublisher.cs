using System;
using System.Buffers;
using System.Collections.Generic;

namespace IT.Messaging;

public interface IMemoryPublisher : IAsyncMemoryPublisher
{
    long Publish(ReadOnlyMemory<byte> message, string? key = null);

    long Publish(IEnumerable<ReadOnlyMemory<byte>> messages, string? key = null);

    long Publish(in ReadOnlySequence<byte> messages, string? key = null);
}