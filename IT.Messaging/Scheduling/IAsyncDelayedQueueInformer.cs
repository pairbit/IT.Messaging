using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncDelayedQueueInformer : IAsyncQueueInformer
{
    Task<long> GetLengthRangeAsync(long? minDelay, long? maxDelay, string? queue = null);
}