using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueInformer
{
    Task<bool> ExistsAsync(string? queue = null);

    Task<long> ExistsAllAsync(IEnumerable<string>? queues = null);

    Task<long> GetLengthAsync(string? queue = null);
}