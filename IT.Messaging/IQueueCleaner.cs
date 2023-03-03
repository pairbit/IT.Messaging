namespace IT.Messaging;

public interface IQueueCleaner : IAsyncQueueCleaner
{
    bool Clean(string? queue = null);

    long CleanAll(string[]? queues = null);
}