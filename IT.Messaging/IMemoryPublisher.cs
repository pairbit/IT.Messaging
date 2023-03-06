using System;
using System.Buffers;

namespace IT.Messaging;

public interface IMemoryPublisher : IAsyncMemoryPublisher
{
    long Publish(ReadOnlyMemory<byte> message, string? key = null);

    long Publish(ReadOnlyMemory<byte>[] messages, string? key = null);

    long Publish(in ReadOnlySequence<byte> messages, string? key = null);
}