﻿using IT.Messaging.Redis.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class MemoryQueueSubscriber : IMemorySubscriber
{
    private const string QueueDefault = "default";
    private readonly IDatabase _db;
    private readonly Dictionary<string?, Tasks> _queues = new();
    private readonly Func<string?, QueueSubscriberConfig> _getConfig;
    private readonly CancellationTokenSource _tokenSource;
    private readonly object _lock = new();

    public MemoryQueueSubscriber(IDatabase db, Func<string?, QueueSubscriberConfig> getConfig)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _getConfig = getConfig;
        _tokenSource = new CancellationTokenSource();
    }

    public void Subscribe(MemoryHandler? handler, MemoryBatchHandler? batchHandler = null, string? queue = null)
    {
        if (handler == null && batchHandler == null) throw new ArgumentNullException(nameof(handler));

        var queues = _queues;

        if (queues.ContainsKey(queue)) throw new HandlerRegisteredException(queue);

        lock (_lock)
        {
            if (queues.ContainsKey(queue)) throw new HandlerRegisteredException(queue);

            var options = _getConfig(queue);
            var delay = options.Delay;
            var batchSize = options.BatchSize;

            var tasks = new Task[options.Tasks];
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            var token = tokenSource.Token;

            if (batchHandler != null)
            {
                if (handler != null)
                {
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = ProcessAsync(queue, delay, token, handler, batchHandler, batchSize);
                    }
                }
                else
                {
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = ProcessAsync(queue, delay, token, batchHandler, batchSize);
                    }
                }
            }
            else if (handler != null)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, token, handler);
                }
            }

            queues.Add(queue, new Tasks(tasks, tokenSource));
        }
    }

    public Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? queue = null)
    {
        if (handler == null && batchHandler == null) throw new ArgumentNullException(nameof(handler));

        var queues = _queues;

        if (queues.ContainsKey(queue)) throw new HandlerRegisteredException(queue);

        lock (_lock)
        {
            if (queues.ContainsKey(queue)) throw new HandlerRegisteredException(queue);

            var options = _getConfig(queue);
            var delay = options.Delay;
            var batchSize = options.BatchSize;

            var tasks = new Task[options.Tasks];
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            var token = tokenSource.Token;

            if (batchHandler != null)
            {
                if (handler != null)
                {
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = ProcessAsync(queue, delay, token, handler, batchHandler, batchSize);
                    }
                }
                else
                {
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = ProcessAsync(queue, delay, token, batchHandler, batchSize);
                    }
                }
            }
            else if (handler != null)
            {
                //if (batchSize > 1) _log.Warning

                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, token, handler);
                }
            }

            queues.Add(queue, new Tasks(tasks, tokenSource));
        }

        return Task.CompletedTask;
    }

    public void Unsubscribe(string queue)
    {
        if (!_queues.TryGetValue(queue, out var tasks)) throw new HandlerNotRegisteredException(queue);

        try
        {
            tasks.TokenSource.Cancel();
        }
        finally
        {
            Task.WhenAll(tasks.Array).Wait();
        }
    }

    public void UnsubscribeAll()
    {
        _tokenSource.Cancel();
    }

    public Task UnsubscribeAllAsync()
    {
        _tokenSource.Cancel();

        return Task.CompletedTask;
    }

    public async Task UnsubscribeAsync(string queue)
    {
        if (!_queues.TryGetValue(queue, out var tasks)) throw new HandlerNotRegisteredException(queue);

        try
        {
            tasks.TokenSource.Cancel();
        }
        finally
        {
            await Task.WhenAll(tasks.Array).ConfigureAwait(false);
            //await Task.WhenAny(Task.WhenAll(tasks.Array), Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);

            _queues.Remove(queue);
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, AsyncMemoryHandler handler)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);

        while (!token.IsCancellationRequested)
        {
            var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var isDelete = true;
                try
                {
                    isDelete = await handler(message, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    isDelete = false;
                }
                finally
                {
                    if (isDelete) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, AsyncMemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            if (batch.Count == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var isDelete = Batch.True;
                try
                {
                    isDelete = await batchHandler(batch, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    isDelete = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(isDelete, Batch.True)) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(isDelete, Batch.False)) await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    else
                    {
                        throw new NotImplementedException();
                        //await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    }
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, AsyncMemoryHandler handler, AsyncMemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            var len = batch.Count;

            if (len == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else if (len == 1)
            {
                var isDelete = true;
                try
                {
                    isDelete = await handler(batch[0], queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    isDelete = false;
                }
                finally
                {
                    if (isDelete) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                }
            }
            else
            {
                var isDelete = Batch.True;
                try
                {
                    isDelete = await batchHandler(batch, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    isDelete = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(isDelete, Batch.True)) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(isDelete, Batch.False)) await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    else
                    {
                        throw new NotImplementedException();
                        //await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    }
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, MemoryHandler handler)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);

        while (!token.IsCancellationRequested)
        {
            var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var isDelete = true;
                try
                {
                    isDelete = handler(message, queue, token);
                }
                catch (OperationCanceledException)
                {
                    isDelete = false;
                }
                finally
                {
                    if (isDelete) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, MemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            if (batch.Count == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var isDelete = Batch.True;
                try
                {
                    isDelete = batchHandler(batch, queue, token);
                }
                catch (OperationCanceledException)
                {
                    isDelete = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(isDelete, Batch.True)) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(isDelete, Batch.False)) await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    else
                    {
                        throw new NotImplementedException();
                        //await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    }
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, MemoryHandler handler, MemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            var len = batch.Count;

            if (len == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else if (len == 1)
            {
                var isDelete = true;
                try
                {
                    isDelete = handler(batch[0], queue, token);
                }
                catch (OperationCanceledException)
                {
                    isDelete = false;
                }
                finally
                {
                    if (isDelete) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                }
            }
            else
            {
                var isDelete = Batch.True;
                try
                {
                    isDelete = batchHandler(batch, queue, token);
                }
                catch (OperationCanceledException)
                {
                    isDelete = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(isDelete, Batch.True)) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(isDelete, Batch.False)) await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    else
                    {
                        throw new NotImplementedException();
                        //await _db.ScriptEvaluateAsync(Lua.QueueRollback, new RedisKey[] { queueWorkingKey, queueKey }).ConfigureAwait(false);
                    }
                    batch.Clear();
                }
            }
        }
    }

    private RedisKey GetQueueKey(string? queue)
    {
        return queue;
    }

    private RedisKey GetQueueWorkingKey(string? queue)
    {
        return queue;
    }
}