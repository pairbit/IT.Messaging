namespace IT.Messaging.Scheduling;

public interface IReadOnlyDelayedQueue : IAsyncReadOnlyDelayedQueue, IMemoryReadOnlyDelayedQueue
{
    long GetDelay<T>(T message, string? queue = null);

    MessageDelay<T>[] GetRange<T>(long? minDelay, long? maxDelay, bool withDelay = false, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}