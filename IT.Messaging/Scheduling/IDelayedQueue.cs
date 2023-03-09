namespace IT.Messaging.Scheduling;

public interface IDelayedQueue : IAsyncDelayedQueue, IReadOnlyDelayedQueue, IMemoryDelayedQueue, IDelayedPublisher
{
    bool Delete<T>(T message, string? queue = null);
}