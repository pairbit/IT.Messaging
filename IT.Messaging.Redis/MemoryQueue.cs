using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class MemoryQueue : IMemoryQueue
{
    protected readonly IDatabase _db;
    private readonly IMemorySubscriber _subscriber;

    public MemoryQueue(IDatabase db, IMemorySubscriber subscriber)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
    }

    #region Public Methods

    public bool Clean(string? queue = null)
    {
        try
        {
            return _db.KeyDelete(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long CleanAll(string[]? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = new RedisKey[queues.Length];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = GetRedisKey(queues[i]);
        }

        try
        {
            return _db.KeyDelete(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> CleanAllAsync(string[]? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = new RedisKey[queues.Length];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = GetRedisKey(queues[i]);
        }

        try
        {
            return _db.KeyDeleteAsync(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<bool> CleanAsync(string? queue = null)
    {
        try
        {
            return _db.KeyDeleteAsync(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Delete(ReadOnlyMemory<byte> message, long count = 0, string? queue = null)
    {
        try
        {
            return _db.ListRemove(GetRedisKey(queue), message, count);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> DeleteAsync(ReadOnlyMemory<byte> message, long count = 0, string? queue = null)
    {
        try
        {
            return _db.ListRemoveAsync(GetRedisKey(queue), message, count);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public bool Exists(string? queue = null)
    {
        try
        {
            return _db.KeyExists(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long ExistsAll(string[]? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = new RedisKey[queues.Length];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = GetRedisKey(queues[i]);
        }

        try
        {
            return _db.KeyExists(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> ExistsAllAsync(string[]? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = new RedisKey[queues.Length];

        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = GetRedisKey(queues[i]);
        }

        try
        {
            return _db.KeyExistsAsync(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<bool> ExistsAsync(string? queue = null)
    {
        try
        {
            return _db.KeyExistsAsync(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long GetLength(string? queue = null)
    {
        try
        {
            return _db.ListLength(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> GetLengthAsync(string? queue = null)
    {
        try
        {
            return _db.ListLengthAsync(GetRedisKey(queue));
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long GetPosition(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPosition(GetRedisKey(queue), message, rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> GetPositionAsync(ReadOnlyMemory<byte> message, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPositionAsync(GetRedisKey(queue), message, rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    #region IMemoryPublisher

    public long Publish(ReadOnlyMemory<byte> message, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);
        try
        {
            return leftPush ? _db.ListLeftPush(redisKey, message) : _db.ListRightPush(redisKey, message);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Publish(IEnumerable<ReadOnlyMemory<byte>> messages, string? key = null)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages));

        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValues = ToRedisValues(messages);
        try
        {
            return leftPush ? _db.ListLeftPush(redisKey, redisValues) : _db.ListRightPush(redisKey, redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Publish(in ReadOnlySequence<byte> messages, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);

        if (messages.IsSingleSegment)
        {
            try
            {
                return leftPush ? _db.ListLeftPush(redisKey, messages.First) : _db.ListRightPush(redisKey, messages.First);
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
        else
        {
            var redisValues = new RedisValue[messages.Length];
            var i = 0;
            foreach (var message in messages)
            {
                redisValues[i++] = message;
            }
            try
            {
                return leftPush ? _db.ListLeftPush(redisKey, redisValues) : _db.ListRightPush(redisKey, redisValues);
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
    }

    public Task<long> PublishAsync(ReadOnlyMemory<byte> message, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);
        try
        {
            return leftPush ? _db.ListLeftPushAsync(redisKey, message) : _db.ListRightPushAsync(redisKey, message);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync(IEnumerable<ReadOnlyMemory<byte>> messages, string? key = null)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages));

        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValues = ToRedisValues(messages);
        try
        {
            return leftPush ? _db.ListLeftPushAsync(redisKey, redisValues) : _db.ListRightPushAsync(redisKey, redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync(in ReadOnlySequence<byte> messages, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);

        if (messages.IsSingleSegment)
        {
            try
            {
                return leftPush ? _db.ListLeftPushAsync(redisKey, messages.First) : _db.ListRightPushAsync(redisKey, messages.First);
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
        else
        {
            var redisValues = new RedisValue[messages.Length];
            var i = 0;
            foreach (var message in messages)
            {
                redisValues[i++] = message;
            }
            try
            {
                return leftPush ? _db.ListLeftPushAsync(redisKey, redisValues) : _db.ListRightPushAsync(redisKey, redisValues);
            }
            catch (RedisException ex)
            {
                throw new MessagingException(null, ex);
            }
        }
    }

    #endregion IMemoryPublisher

    #region IMemorySubscriber

    public void Subscribe(MemoryHandler? handler, MemoryBatchHandler? batchHandler = null, string? key = null) => _subscriber.Subscribe(handler, batchHandler, key);

    public Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryBatchHandler? batchHandler = null, string? key = null) => _subscriber.SubscribeAsync(handler, batchHandler, key);

    public void Unsubscribe(string key) => _subscriber.Unsubscribe(key);

    public void UnsubscribeAll() => _subscriber.UnsubscribeAll();

    public Task UnsubscribeAllAsync() => _subscriber.UnsubscribeAllAsync();

    public Task UnsubscribeAsync(string key) => _subscriber.UnsubscribeAsync(key);

    #endregion IMemorySubscriber

    #endregion Public Methods

    #region Protected Methods

    protected RedisKey GetQueueKey(string? key, out bool leftPush)
    {
        leftPush = true;
        return key ?? "default";
    }

    protected RedisKey GetRedisKey(string? queue)
    {
        return queue ?? "default";
    }

    #endregion Protected Methods

    private static RedisValue[] ToRedisValues(IEnumerable<ReadOnlyMemory<byte>> messages)
    {
        RedisValue[] redisValues;

        if (messages is IReadOnlyList<ReadOnlyMemory<byte>> readOnlyList)
        {
            redisValues = new RedisValue[readOnlyList.Count];

            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = readOnlyList[i];
            }
        }
        else if (messages is IList<ReadOnlyMemory<byte>> list)
        {
            redisValues = new RedisValue[list.Count];

            for (int i = 0; i < redisValues.Length; i++)
            {
                redisValues[i] = list[i];
            }
        }
        else if (messages is IReadOnlyCollection<ReadOnlyMemory<byte>> readOnlyCollection)
        {
            redisValues = new RedisValue[readOnlyCollection.Count];

            var i = 0;

            foreach (var message in messages)
            {
                redisValues[i++] = message;
            }
        }
        else if (messages is ICollection<ReadOnlyMemory<byte>> messageCollection)
        {
            redisValues = new RedisValue[messageCollection.Count];

            var i = 0;

            foreach (var message in messages)
            {
                redisValues[i++] = message;
            }
        }
        //else if (messages is ICollection collection)
        //{
        //    redisValues = new RedisValue[collection.Count];

        //    var i = 0;

        //    foreach (var message in messages)
        //    {
        //        redisValues[i++] = message;
        //    }
        //}
        else
        {
            var redisValueList = new List<RedisValue>();
            foreach (var message in messages)
            {
                redisValueList.Add(message);
            }
            redisValues = redisValueList.ToArray();
        }

        return redisValues;
    }
}