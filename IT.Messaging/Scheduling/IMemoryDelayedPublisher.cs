using System;
using System.Collections.Generic;

namespace IT.Messaging.Scheduling;

public interface IMemoryDelayedPublisher : IAsyncMemoryDelayedPublisher
{
    bool DelayPublish(long delay, ReadOnlyMemory<byte> message, string? channel = null);

    bool DelayPublish(long delay, IEnumerable<ReadOnlyMemory<byte>> messages, string? channel = null);

    long DelayPublish(IEnumerable<MessageDelay> messages, string? channel = null);
}