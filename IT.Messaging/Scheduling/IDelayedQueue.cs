using System.Collections.Generic;

namespace IT.Messaging.Scheduling;

public interface IDelayedQueue : IAsyncDelayedQueue, IReadOnlyDelayedQueue, IMemoryDelayedQueue, IDelayedPublisher
{
    bool Delete<T>(T message, string? queue = null);

    long Delete<T>(IEnumerable<T> messages, string? queue = null);
}