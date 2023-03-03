using System;
using System.Threading.Tasks;

namespace IT.Messaging.Scheduling;

public interface IAsyncMemoryDelayedPublisher
{
    Task<bool> DelayPublishAsync(long delay, ReadOnlyMemory<byte> message, string? channel = null);

    Task<bool> DelayPublishAsync(long delay, ReadOnlyMemory<byte>[] messages, string? channel = null);

    Task<long> DelayPublishAsync(MessageDelay[] messages, string? channel = null);
}