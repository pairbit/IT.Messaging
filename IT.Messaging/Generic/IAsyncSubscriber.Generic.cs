using System;
using System.Threading.Tasks;

namespace IT.Messaging.Generic;

public interface IAsyncSubscriber<T> : IAsyncUnsubscriber
{
    Task SubscribeAsync(Action<T, string?> handler, string? channel = null);

    Task SubscribeAsync(Action<T[], string?> handler, string? channel = null);
}