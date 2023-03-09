using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncDelayedQueueCleaner : IAsyncQueueCleaner
{
    Task<long> CleanRangeAsync(long? minDelay, long? maxDelay, string? queue = null);
}