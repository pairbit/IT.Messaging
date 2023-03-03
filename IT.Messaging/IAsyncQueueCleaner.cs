using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueCleaner
{
    Task<bool> CleanAsync(string? queue = null);

    Task<long> CleanAllAsync(string[]? queues = null);
}