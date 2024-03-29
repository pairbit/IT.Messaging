﻿namespace IT.Messaging;

public interface IReadOnlyQueue : IAsyncReadOnlyQueue, IMemoryReadOnlyQueue
{
    long GetPosition<T>(T message, long rank = 1, long maxLength = 0, string? queue = null);

    long[] GetPositions<T>(T message, long count, long rank = 1, long maxLength = 0, string? queue = null);

    T? GetByIndex<T>(long index, string? queue = null);

    T[] GetRange<T>(long min = 0, long max = -1, string? queue = null);
}