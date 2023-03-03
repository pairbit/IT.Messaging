﻿using System;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryReadOnlyQueue : IAsyncQueueInformer
{
    Task<long> GetPositionAsync(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null);
}