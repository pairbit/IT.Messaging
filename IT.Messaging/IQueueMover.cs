namespace IT.Messaging;

public interface IQueueMover : IAsyncQueueMover
{
    bool Rename(string newQueue, string? queue = null);

    long ListMoveAll(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null);
}