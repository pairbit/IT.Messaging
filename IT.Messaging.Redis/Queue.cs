using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

using Internal;

public class Queue : MemoryQueue, IQueue
{
    private readonly ISubscriber _subscriber;
    private readonly IRedisValueSerializer _serializer;

    public Queue(
        IDatabase db,
        IRedisValueSerializer serializer,
        ISubscriber subscriber) : base(db, subscriber)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _subscriber = subscriber;
    }

    public long Delete<T>(T message, long count = 0, string? queue = null)
    {
        try
        {
            return _db.ListRemove(GetRedisKey(queue), _serializer.Serialize(message), count);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Delete<T>(IEnumerable<T> messages, long count = 0, string? queue = null)
    {
        var redisValues = messages.ToRedisValues(_serializer, 1);
        redisValues[0] = count;
        try
        {
            return _db.ListRemoveAll(GetRedisKey(queue), redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> DeleteAsync<T>(T message, long count = 0, string? queue = null)
    {
        try
        {
            return _db.ListRemoveAsync(GetRedisKey(queue), _serializer.Serialize(message), count);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> DeleteAsync<T>(IEnumerable<T> messages, long count = 0, string? queue = null)
    {
        var redisValues = messages.ToRedisValues(_serializer, 1);
        redisValues[0] = count;
        try
        {
            return _db.ListRemoveAllAsync(GetRedisKey(queue), redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long GetPosition<T>(T message, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPosition(GetRedisKey(queue), _serializer.Serialize(message), rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> GetPositionAsync<T>(T message, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPositionAsync(GetRedisKey(queue), _serializer.Serialize(message), rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    #region IPublisher

    public long Publish<T>(T message, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValue = _serializer.Serialize(message);
        try
        {
            return leftPush ? _db.ListLeftPush(redisKey, redisValue) : _db.ListRightPush(redisKey, redisValue);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long Publish<T>(IEnumerable<T> messages, string? key = null)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages));

        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValues = messages.ToRedisValues(_serializer);
        try
        {
            return leftPush ? _db.ListLeftPush(redisKey, redisValues) : _db.ListRightPush(redisKey, redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync<T>(T message, string? key = null)
    {
        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValue = _serializer.Serialize(message);
        try
        {
            return leftPush ? _db.ListLeftPushAsync(redisKey, redisValue) : _db.ListRightPushAsync(redisKey, redisValue);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> PublishAsync<T>(IEnumerable<T> messages, string? key = null)
    {
        if (messages == null) throw new ArgumentNullException(nameof(messages));

        var redisKey = GetQueueKey(key, out var leftPush);
        var redisValues = messages.ToRedisValues(_serializer);
        try
        {
            return leftPush ? _db.ListLeftPushAsync(redisKey, redisValues) : _db.ListRightPushAsync(redisKey, redisValues);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    #endregion IPublisher

    #region ISubscriber

    public void Subscribe<T>(Handler<T>? handler, BatchHandler<T>? batchHandler = null, string? key = null) => _subscriber.Subscribe(handler, batchHandler, key);

    public Task SubscribeAsync<T>(AsyncHandler<T>? handler, AsyncBatchHandler<T>? batchHandler = null, string? key = null) => _subscriber.SubscribeAsync(handler, batchHandler, key);

    #endregion ISubscriber
}