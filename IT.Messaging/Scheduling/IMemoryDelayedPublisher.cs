using System;

namespace IT.Messaging.Scheduling;

public interface IMemoryDelayedPublisher : IAsyncMemoryDelayedPublisher
{
    bool DelayPublish(long delay, ReadOnlyMemory<byte> message, string? channel = null);

    bool DelayPublish(long delay, ReadOnlyMemory<byte>[] messages, string? channel = null);

    long DelayPublish(MessageDelay[] messages, string? channel = null);
}