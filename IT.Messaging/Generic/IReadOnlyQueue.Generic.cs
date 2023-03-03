namespace IT.Messaging.Generic;

public interface IReadOnlyQueue<T> : IAsyncReadOnlyQueue<T>, IQueueInformer
{
    long GetPosition(T message, long rank = 1, long maxLength = 0, string? queue = null);
}