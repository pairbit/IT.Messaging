using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemorySubscriber : IAsyncUnsubscriber
{
    Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null);

    Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null);
}