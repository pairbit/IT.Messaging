using System.Collections.Generic;

namespace IT.Messaging.Scheduling;

public interface IDelayedPublisher : IAsyncDelayedPublisher, IMemoryDelayedPublisher
{
    bool DelayPublish<T>(long delay, T message, string? channel = null);

    bool DelayPublish<T>(long delay, IEnumerable<T> messages, string? channel = null);

    long DelayPublish<T>(IEnumerable<MessageDelay<T>> messages, string? channel = null);
}