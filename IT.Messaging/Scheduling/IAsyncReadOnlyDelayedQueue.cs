using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncReadOnlyDelayedQueue : IAsyncMemoryReadOnlyDelayedQueue
{
    Task<long> GetDelayAsync<T>(T message, string? queue = null);

    Task<MessageDelay<T>[]> GetRangeAsync<T>(long? minDelay, long? maxDelay, bool withDelay = false, bool ascending = true, long skip = 0, long take = -1, string? queue = null);
}