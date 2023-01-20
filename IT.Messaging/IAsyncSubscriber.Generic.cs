using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncSubscriber<T> : IAsyncUnsubscriber
{
    Task SubscribeAsync(Action<T, string?> handler, string? channel = null);
}