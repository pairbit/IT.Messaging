using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncReadOnlyDelayedQueue : IAsyncMemoryReadOnlyDelayedQueue
{
    Task<long> GetDelayAsync<T>(T message, string? queue = null);

    Task<long[]> GetDelayAsync<T>(IEnumerable<T> messages, string? queue = null);

    Task<MessageDelay<T>[]> GetRangeAsync<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);

    Task<T[]> GetMessageRangeAsync<T>(long minDelay, long maxDelay, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}