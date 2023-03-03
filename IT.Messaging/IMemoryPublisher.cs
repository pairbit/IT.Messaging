using System;

namespace IT.Messaging;

public interface IMemoryPublisher : IAsyncMemoryPublisher
{
    long Publish(ReadOnlyMemory<byte> message, string? key = null);

    long Publish(ReadOnlyMemory<byte>[] messages, string? key = null);
}