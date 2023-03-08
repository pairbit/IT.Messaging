using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Messaging;

public delegate Task<bool> AsyncMemoryHandler(ReadOnlyMemory<byte> message, string? queue, CancellationToken token);
public delegate Task<bool[]> AsyncMemoryBatchHandler(IReadOnlyList<ReadOnlyMemory<byte>> messages, string? queue, CancellationToken token);
//public delegate Task<bool[]> AsyncSequenceHandler(ReadOnlySequence<byte> messages, string? queue, CancellationToken token);
public delegate Task<bool> AsyncHandler<T>(T message, string? queue, CancellationToken token);
public delegate Task<bool[]> AsyncBatchHandler<T>(IReadOnlyList<T> messages, string? queue, CancellationToken token);

public delegate bool MemoryHandler(ReadOnlyMemory<byte> message, string? queue, CancellationToken token);
public delegate bool[] MemoryBatchHandler(IReadOnlyList<ReadOnlyMemory<byte>> messages, string? queue, CancellationToken token);//Batch
//public delegate bool[] SequenceHandler(ReadOnlySequence<byte> messages, string? queue, CancellationToken token);
public delegate bool Handler<T>(T message, string? queue, CancellationToken token);
public delegate bool[] BatchHandler<T>(IReadOnlyList<T> messages, string? queue, CancellationToken token);