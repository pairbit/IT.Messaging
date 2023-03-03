namespace IT.Messaging;

public interface IQueueInformer : IAsyncQueueInformer
{
    long GetLength(string? queue = null);
}