using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncQueue : IAsyncMemoryQueue, IAsyncReadOnlyQueue, IAsyncChannel
{
    Task<long> DeleteAsync<T>(T message, long count = 0, string? queue = null);

    Task<long> DeleteAsync<T>(IEnumerable<T> messages, long count = 0, string? queue = null);
}