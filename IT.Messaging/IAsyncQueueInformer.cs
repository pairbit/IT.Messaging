using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueInformer
{
    Task<bool> ExistsAsync(string? queue = null);

    Task<long> ExistsAllAsync(string[]? queues = null);

    Task<long> GetLengthAsync(string? queue = null);
}