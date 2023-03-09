using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Scheduling.Generic;

public interface IAsyncDelayedPublisher<T>
{
    Task<bool> DelayPublishAsync(long delay, T message, string? channel = null);

    Task<bool> DelayPublishAsync(long delay, IEnumerable<T> messages, string? channel = null);

    Task<long> DelayPublishAsync(IEnumerable<MessageDelay<T>> messages, string? channel = null);
}