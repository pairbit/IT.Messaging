namespace IT.Messaging;

public interface IQueueInformer : IAsyncQueueInformer
{
    bool Exists(string? queue = null);

    long ExistsAll(string[]? queues = null);

    long GetLength(string? queue = null);
}