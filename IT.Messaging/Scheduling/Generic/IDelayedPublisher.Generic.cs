namespace IT.Messaging.Scheduling.Generic;

public interface IDelayedPublisher<T> : IAsyncDelayedPublisher<T>
{
    bool DelayPublish(long delay, T message, string? channel = null);

    bool DelayPublish(long delay, T[] messages, string? channel = null);

    long DelayPublish(MessageDelay<T>[] messages, string? channel = null);
}