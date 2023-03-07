namespace IT.Messaging;

public interface IMemorySubscriber : IAsyncMemorySubscriber, IUnsubscriber
{
    void Subscribe(MemoryHandler? handler, MemoryBatchHandler? batchHandler = null, string? key = null);

    //void Subscribe(MemoryHandler handler, SequenceHandler batchHandler, string? key = null);
}