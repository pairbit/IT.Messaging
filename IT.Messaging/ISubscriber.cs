using System;

namespace IT.Messaging;

public interface ISubscriber : IAsyncSubscriber, IMemorySubscriber
{
    void Subscribe<T>(Action<T, string?> handler, string? channel = null);
}