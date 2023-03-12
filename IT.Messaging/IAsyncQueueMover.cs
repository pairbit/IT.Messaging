using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueMover
{
    Task<bool> RenameAsync(string newQueue, string? queue = null);

    Task<long> ListMoveAllAsync(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null);
}