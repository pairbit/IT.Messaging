using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncDelayedPublisher : IAsyncMemoryDelayedPublisher
{
    Task<bool> DelayPublishAsync<T>(long delay, T message, string? channel = null);

    Task<bool> DelayPublishAsync<T>(long delay, T[] messages, string? channel = null);

    Task<long> DelayPublishAsync<T>(MessageDelay<T>[] messages, string? channel = null);
}