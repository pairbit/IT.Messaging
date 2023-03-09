using System.Collections.Generic;

namespace IT.Messaging.Generic;

public interface IPublisher<T> : IAsyncPublisher<T>
{
    long Publish(T message, string? key = null);

    long Publish(IEnumerable<T> messages, string? key = null);
}