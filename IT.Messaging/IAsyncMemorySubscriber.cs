using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemorySubscriber : IAsyncUnsubscriber
{
    Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? key = null);

    Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? key = null);

    //Task SubscribeAsync(Action<System.Buffers.ReadOnlySequence<byte>, string?> handler, string? key = null);
}