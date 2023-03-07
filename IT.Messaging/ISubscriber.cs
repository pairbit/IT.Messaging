namespace IT.Messaging;

public interface ISubscriber : IAsyncSubscriber, IMemorySubscriber
{
    void Subscribe<T>(Handler<T>? handler, BatchHandler<T>? batchHandler = null, string? key = null);
}