using System;

namespace IT.Messaging;

public interface IMemorySubscriber : IAsyncMemorySubscriber, IUnsubscriber
{
    void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? key = null);

    void Subscribe(Action<ReadOnlyMemory<byte>[], string?> handler, string? key = null);

    //void Subscribe(Action<System.Buffers.ReadOnlySequence<byte>, string?> handler, string? key = null);
}