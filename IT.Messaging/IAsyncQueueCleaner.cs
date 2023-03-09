using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueueCleaner
{
    Task<bool> CleanAsync(string? queue = null);

    Task<long> CleanAllAsync(IEnumerable<string>? queues = null);
}