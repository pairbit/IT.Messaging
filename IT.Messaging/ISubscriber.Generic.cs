using System;

namespace IT.Messaging;

public interface ISubscriber<T> : IAsyncSubscriber<T>, IUnsubscriber
{
    void Subscribe(Action<T, string?> handler, string? channel = null);
}