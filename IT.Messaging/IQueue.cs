using System.Collections.Generic;

namespace IT.Messaging;

public interface IQueue : IAsyncQueue, IMemoryQueue, IReadOnlyQueue, IChannel
{
    //DeleteRange

    long Delete<T>(T message, long count = 0, string? queue = null);

    long Delete<T>(IEnumerable<T> messages, long count = 0, string? queue = null);
}