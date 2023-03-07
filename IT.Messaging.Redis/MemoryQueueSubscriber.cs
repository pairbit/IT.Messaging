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
    private List<string?> _queues = new();
    private Func<string?, QueueSubscriberConfig> _getConfig;
    private CancellationTokenSource _tokenSource;

    public MemoryQueueSubscriber(IDatabase db, Func<string?, QueueSubscriberConfig> getConfig)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _getConfig = getConfig;
    }

    public void Subscribe(MemoryHandler? handler, MemoryBatchHandler? batchHandler = null, string? queue = null)
    {
        if (handler == null && batchHandler == null) throw new ArgumentNullException(nameof(handler));

        var queues = _queues;

        if (queues.Contains(queue)) throw new HandlerRegisteredException(queue);

        queues.Add(queue);

        var options = _getConfig(queue);
        var delay = options.Delay;
        var batchSize = options.BatchSize;

        var tasks = new Task[options.Tasks];

        if (batchHandler != null)
        {
            if (handler != null)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, handler, batchHandler, batchSize);
                }
            }
            else
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, batchHandler, batchSize);
                }
            }
        }
        else if (handler != null)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ProcessAsync(queue, delay, handler);
            }
        }
    }

    public Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? queue = null)
    {
        if (handler == null && batchHandler == null) throw new ArgumentNullException(nameof(handler));

        var queues = _queues;

        if (queues.Contains(queue)) throw new HandlerRegisteredException(queue);

        queues.Add(queue);

        var options = _getConfig(queue);
        var delay = options.Delay;
        var batchSize = options.BatchSize;

        var tasks = new Task[options.Tasks];

        if (batchHandler != null)
        {
            if (handler != null)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, handler, batchHandler, batchSize);
                }
            }
            else
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ProcessAsync(queue, delay, batchHandler, batchSize);
                }
            }
        }
        else if (handler != null)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ProcessAsync(queue, delay, handler);
            }
        }

        return Task.CompletedTask;
    }

    public void Unsubscribe(string queue)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeAll()
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task UnsubscribeAsync(string queue)
    {
        throw new NotImplementedException();
    }

    private async Task ProcessAsync(string? queue, int delay, AsyncMemoryHandler handler)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    await handler(message, queue).ConfigureAwait(false);
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, AsyncMemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;
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
                try
                {
                    await batchHandler(batch, queue).ConfigureAwait(false);
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, AsyncMemoryHandler handler, AsyncMemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;
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
            else
            {
                try
                {
                    if (len == 1)
                    {
                        await handler(batch[0], queue).ConfigureAwait(false);
                    }
                    else
                    {
                        await batchHandler(batch, queue).ConfigureAwait(false);
                    }
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, MemoryHandler handler)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            var message = await _db.ListRightPopLeftPushAsync(queueKey, queueWorkingKey).ConfigureAwait(false);

            if (message.IsNull)
            {
                await Task.Delay(delay, token).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    handler(message, queue);
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, MemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;
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
                try
                {
                    batchHandler(batch, queue);
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
                    batch.Clear();
                }
            }
        }
    }

    private async Task ProcessAsync(string? queue, int delay, MemoryHandler handler, MemoryBatchHandler batchHandler, int batchSize)
    {
        var queueKey = GetQueueKey(queue);
        var queueWorkingKey = GetQueueWorkingKey(queue);
        var token = _tokenSource.Token;
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
            else
            {
                try
                {
                    if (len == 1)
                    {
                        handler(batch[0], queue);
                    }
                    else
                    {
                        batchHandler(batch, queue);
                    }
                }
                finally
                {
                    await _db.KeyDeleteAsync(queueWorkingKey).ConfigureAwait(false);
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