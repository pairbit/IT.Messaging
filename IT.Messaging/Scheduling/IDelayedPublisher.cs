namespace IT.Messaging.Scheduling;

public interface IDelayedPublisher : IAsyncDelayedPublisher, IMemoryDelayedPublisher
{
    bool DelayPublish<T>(long delay, T message, string? channel = null);

    bool DelayPublish<T>(long delay, T[] messages, string? channel = null);

    long DelayPublish<T>(MessageDelay<T>[] messages, string? channel = null);
}