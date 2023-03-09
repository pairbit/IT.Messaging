using System.Collections.Generic;

namespace IT.Messaging;

public interface IQueueCleaner : IAsyncQueueCleaner
{
    bool Clean(string? queue = null);

    long CleanAll(IEnumerable<string>? queues = null);
}