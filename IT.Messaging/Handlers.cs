using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Messaging;

public delegate Task AsyncMemoryHandler(ReadOnlyMemory<byte> message, string? queue, CancellationToken token);
public delegate Task AsyncMemoryBatchHandler(IReadOnlyCollection<ReadOnlyMemory<byte>> messages, string? queue, CancellationToken token);
//public delegate Task AsyncSequenceHandler(ReadOnlySequence<byte> messages, string? queue, CancellationToken token);
public delegate Task AsyncHandler<T>(T message, string? queue, CancellationToken token);
public delegate Task AsyncBatchHandler<T>(IReadOnlyCollection<T> messages, string? queue, CancellationToken token);

public delegate void MemoryHandler(ReadOnlyMemory<byte> message, string? queue, CancellationToken token);
public delegate void MemoryBatchHandler(IReadOnlyCollection<ReadOnlyMemory<byte>> messages, string? queue, CancellationToken token);//Batch
//public delegate void SequenceHandler(ReadOnlySequence<byte> messages, string? queue, CancellationToken token);
public delegate void Handler<T>(T message, string? queue, CancellationToken token);
public delegate void BatchHandler<T>(IReadOnlyCollection<T> messages, string? queue, CancellationToken token);