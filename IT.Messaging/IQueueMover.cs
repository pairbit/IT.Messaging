namespace IT.Messaging;

public interface IQueueMover : IAsyncQueueMover
{
    bool Rename(string newQueue, string? queue = null);
}