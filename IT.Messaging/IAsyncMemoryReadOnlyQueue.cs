﻿using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryReadOnlyQueue : IAsyncQueueInformer
{
    Task<long> GetPositionAsync(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null);

    Task<long[]> GetPositionsAsync(ReadOnlyMemory<byte> message, long count, long rank = 1, long maxLength = 0, string? queue = null);

    Task<ReadOnlyMemory<byte>> GetByIndexAsync(long index, string? queue = null);

    Task<ReadOnlyMemory<byte>[]> GetRangeAsync(long min = 0, long max = -1, string? queue = null);
}