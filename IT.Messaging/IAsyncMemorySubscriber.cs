using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemorySubscriber : IAsyncUnsubscriber
{
    Task SubscribeAsync(Action<ReadOnlyMemory<Byte>, string?> handler, string? channel = null);
}