using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueMover
{
    Task<bool> RenameAsync(string newQueue, string? queue = null);
}