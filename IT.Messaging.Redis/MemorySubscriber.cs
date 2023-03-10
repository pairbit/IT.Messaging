using IT.Messaging.Redis.Internal;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

internal delegate Task<RedisValue> AsyncDequeue(IDatabaseAsync db, RedisKey source, RedisKey destination);

public class MemorySubscriber : IMemorySubscriber
{
    private const string QueueDefault = "default";
    private readonly IDatabase _db;
    private readonly Dictionary<string?, Tasks> _queues = new();
    private readonly Func<string?, QueueSubscriberConfig> _getConfig;
    private readonly CancellationTokenSource _tokenSource;
    private readonly object _lock = new();

    private bool? haveMove;
    private AsyncDequeue _dequeueAsync;

    public MemorySubscriber(IDatabase db, Func<string?, QueueSubscriberConfig> getConfig)
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

        Init();

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

    public async Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? queue = null)
    {
        if (handler == null && batchHandler == null) throw new ArgumentNullException(nameof(handler));

        var queues = _queues;

        if (queues.ContainsKey(queue)) throw new HandlerRegisteredException(queue);

        await InitAsync();

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
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var status = true;
                try
                {
                    status = await handler(message, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    status = false;
                }
                finally
                {
                    if (status) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, AsyncMemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            if (batch.Count == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var status = Batch.True;
                try
                {
                    status = await batchHandler(batch, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    status = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(status, Batch.True)) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(status, Batch.False)) await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey, status).ConfigureAwait(false);

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
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

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
                var status = true;
                try
                {
                    status = await handler(batch[0], queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    status = false;
                }
                finally
                {
                    if (status) await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                }
            }
            else
            {
                var status = Batch.True;
                try
                {
                    status = await batchHandler(batch, queue, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    status = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(status, Batch.True)) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(status, Batch.False)) await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey, status).ConfigureAwait(false);

                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, MemoryHandler handler)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var status = true;
                try
                {
                    status = handler(message, queue, token);
                }
                catch (OperationCanceledException)
                {
                    status = false;
                }
                finally
                {
                    if (status) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, CancellationToken token, MemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var batch = new List<ReadOnlyMemory<byte>>(batchSize);
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

                if (message.IsNull) break;

                batch.Add(message);
            }

            if (batch.Count == 0)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                var status = Batch.True;
                try
                {
                    status = batchHandler(batch, queue, token);
                }
                catch (OperationCanceledException)
                {
                    status = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(status, Batch.True)) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(status, Batch.False)) await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey, status).ConfigureAwait(false);

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
        var db = _db;
        var dequeueAsync = _dequeueAsync;

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < batchSize; i++)
            {
                var message = await dequeueAsync(db, queueKey, queueWorkingKey).ConfigureAwait(false);

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
                var status = true;
                try
                {
                    status = handler(batch[0], queue, token);
                }
                catch (OperationCanceledException)
                {
                    status = false;
                }
                finally
                {
                    if (status) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                }
            }
            else
            {
                var status = Batch.True;
                try
                {
                    status = batchHandler(batch, queue, token);
                }
                catch (OperationCanceledException)
                {
                    status = Batch.False;
                }
                finally
                {
                    if (ReferenceEquals(status, Batch.True)) await db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    else if (ReferenceEquals(status, Batch.False)) await db.QueueRollbackAsync(queueWorkingKey, queueKey).ConfigureAwait(false);
                    else await db.QueueRollbackAsync(queueWorkingKey, queueKey, status).ConfigureAwait(false);
                    batch.Clear();
                }
            }
        }
    }

    private void Init()
    {
        if (haveMove == null)
        {
            try
            {
                var source = GetQueueKey("5e2e8749395a46f7acd0e7f4a087e3ab-source");
                var destination = GetQueueKey("5e2e8749395a46f7acd0e7f4a087e3ab-destination");

                try
                {
                    _db.ListLeftPush(source, 1);
                    _db.ListMove(source, destination, ListSide.Right, ListSide.Left);
                    haveMove = true;
                    _dequeueAsync = ListMoveAsync;
                }
                catch (RedisServerException ex)
                {
                    var message = ex.Message;
                    if (message != null && message.Equals("ERR unknown command 'LMOVE'"))
                    {
                        haveMove = false;
                        _dequeueAsync = ListRightPopLeftPushAsync;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    _db.KeyDelete(new[] { source, destination });
                }
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
    }

    private async Task InitAsync()
    {
        if (haveMove == null)
        {
            try
            {
                var source = GetQueueKey("5e2e8749395a46f7acd0e7f4a087e3ab-source");
                var destination = GetQueueKey("5e2e8749395a46f7acd0e7f4a087e3ab-destination");

                try
                {
                    await _db.ListLeftPushAsync(source, 1).ConfigureAwait(false);
                    await _db.ListMoveAsync(source, destination, ListSide.Right, ListSide.Left).ConfigureAwait(false);
                    haveMove = true;
                    _dequeueAsync = ListMoveAsync;
                }
                catch (RedisServerException ex)
                {
                    var message = ex.Message;
                    if (message != null && message.Equals("ERR unknown command 'LMOVE'"))
                    {
                        haveMove = false;
                        _dequeueAsync = ListRightPopLeftPushAsync;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    await _db.KeyDeleteAsync(new[] { source, destination }).ConfigureAwait(false);
                }
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
    }

    private static Task<RedisValue> ListMoveAsync(IDatabaseAsync db, RedisKey source, RedisKey destination)
        => db.ListMoveAsync(source, destination, ListSide.Right, ListSide.Left);

    private static Task<RedisValue> ListRightPopLeftPushAsync(IDatabaseAsync db, RedisKey source, RedisKey destination)
        => db.ListRightPopLeftPushAsync(source, destination);

    private RedisKey GetQueueKey(string? queue)
    {
        return queue;
    }

    private RedisKey GetQueueWorkingKey(string? queue)
    {
        return queue;
    }
}