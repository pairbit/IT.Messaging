using System;

namespace IT.Messaging;

public interface IMemorySubscriber : IAsyncMemorySubscriber
{
    void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null);
}