namespace IT.Messaging;

public interface IReadOnlyQueue : IAsyncReadOnlyQueue, IMemoryReadOnlyQueue
{
    long GetPosition<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);

    T? GetByIndex<T>(long index, string? queue = null);

    T[] GetRange<T>(long start = 0, long stop = -1, string? queue = null);
}