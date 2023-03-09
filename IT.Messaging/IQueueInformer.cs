using System.Collections.Generic;

namespace IT.Messaging;

public interface IQueueInformer : IAsyncQueueInformer
{
    bool Exists(string? queue = null);

    long ExistsAll(IEnumerable<string>? queues = null);

    long GetLength(string? queue = null);
}