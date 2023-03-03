using System.Threading.Tasks;

namespace IT.Messaging.Scheduling.Generic;

public interface IAsyncDelayedPublisher<T>
{
    Task<bool> DelayPublishAsync(long delay, T message, string? channel = null);

    Task<bool> DelayPublishAsync(long delay, T[] messages, string? channel = null);

    Task<long> DelayPublishAsync(MessageDelay<T>[] messages, string? channel = null);
}