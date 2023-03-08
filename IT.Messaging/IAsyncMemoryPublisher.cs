using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryPublisher
{
    Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? key = null);

    Task<long> PublishAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? key = null);

    Task<long> PublishAsync(in ReadOnlySequence<byte> messages, string? key = null);
}