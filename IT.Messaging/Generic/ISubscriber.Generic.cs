namespace IT.Messaging.Generic;

public interface ISubscriber<T> : IAsyncSubscriber<T>, IUnsubscriber
{
    void Subscribe(Handler<T>? handler, BatchHandler<T>? batchHandler = null, string? key = null);
}