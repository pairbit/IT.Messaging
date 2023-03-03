using System;

namespace IT.Messaging.Generic;

public interface ISubscriber<T> : IAsyncSubscriber<T>, IUnsubscriber
{
    void Subscribe(Action<T, string?> handler, string? key = null);

    void Subscribe(Action<T[], string?> handler, string? key = null);
}