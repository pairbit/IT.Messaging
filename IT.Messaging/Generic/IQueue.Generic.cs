namespace IT.Messaging.Generic;

public interface IQueue<T> : IAsyncQueue<T>, IChannel<T>, IReadOnlyQueue<T>, IQueueCleaner
{
    long Delete(T message, long count = 0, string? queue = null);
}