using System;

namespace IT.Messaging;

public interface IMemoryPublisher : IAsyncMemoryPublisher
{
    long Publish(ReadOnlyMemory<byte> message, string? channel = null);
}