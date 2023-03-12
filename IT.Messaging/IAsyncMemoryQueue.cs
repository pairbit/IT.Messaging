using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging;

public interface IAsyncMemoryQueue : IAsyncMemoryReadOnlyQueue, IAsyncQueueTrimmer, IAsyncQueueMover, IAsyncQueueCleaner, IAsyncMemoryChannel
{
    //Task<ReadOnlyMemory<byte>> MoveAsync(string destinationQueue, Side destinationSide = Side.Left, Side side = Side.Right, string? queue = null);

    Task<long> MoveAllAsync(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null);

    //Task<long> PushAsync(ReadOnlyMemory<byte> message, Side side = Side.Left, string? queue = null);

    //Task<long> PushAsync(ReadOnlyMemory<byte> message, IEnumerable<QueueSide> queues);

    //Task<long> PushAsync(IEnumerable<ReadOnlyMemory<byte>> messages, Side side = Side.Left, string? queue = null);

    //Task<long> PushAsync(IEnumerable<ReadOnlyMemory<byte>> messages, IEnumerable<QueueSide> queues);

    //Task<long> InsertAsync(ReadOnlyMemory<byte> target, ReadOnlyMemory<byte> message, Side side = Side.Left, string? queue = null);

    //Task InsertByIndexAsync(long index, ReadOnlyMemory<byte> message, string? queue = null);

    //Task<ReadOnlyMemory<byte>> PopAsync(Side side = Side.Right, string? queue = null);

    //Task<ReadOnlyMemory<byte>[]> PopAsync(long count, Side side = Side.Right, string? queue = null);

    Task<long> DeleteAsync(ReadOnlyMemory<byte> message, long count = 0, string? queue = null);

    Task<long> DeleteAsync(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null);
}