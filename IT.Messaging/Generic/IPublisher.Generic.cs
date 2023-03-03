namespace IT.Messaging.Generic;

public interface IPublisher<T> : IAsyncPublisher<T>
{
    long Publish(T message, string? key = null);

    long Publish(T[] messages, string? key = null);
}