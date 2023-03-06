//using IT.Messaging.Generic;
//using StackExchange.Redis;
//using System;
//using System.Threading.Tasks;

//namespace IT.Messaging.Redis.Generic;

//public class Queue<T> : IQueue<T>
//{
//    protected readonly IDatabase _db;
//    private readonly ISubscriber<T> _subscriber;
//    private readonly IRedisValueSerializer<T> _serializer;

//    public Queue(
//        IDatabase db,
//        IRedisValueSerializer<T> serializer,
//        ISubscriber<T> subscriber)
//    {
//        _db = db ?? throw new ArgumentNullException(nameof(db));
//        _subscriber = subscriber ?? throw new ArgumentNullException(nameof(subscriber));
//        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
//    }

//    #region Public Methods

//    public bool Clean(string? queue = null)
//    {
//        try
//        {
//            return _db.KeyDelete(GetRedisKey(queue));
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public long CleanAll(string[]? queues = null)
//    {
//        if (queues == null) throw new ArgumentNullException(nameof(queues));

//        var keys = new RedisKey[queues.Length];

//        for (int i = 0; i < keys.Length; i++)
//        {
//            keys[i] = GetRedisKey(queues[i]);
//        }

//        try
//        {
//            return _db.KeyDelete(keys);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> CleanAllAsync(string[]? queues = null)
//    {
//        if (queues == null) throw new ArgumentNullException(nameof(queues));

//        var keys = new RedisKey[queues.Length];

//        for (int i = 0; i < keys.Length; i++)
//        {
//            keys[i] = GetRedisKey(queues[i]);
//        }

//        try
//        {
//            return _db.KeyDeleteAsync(keys);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<bool> CleanAsync(string? queue = null)
//    {
//        try
//        {
//            return _db.KeyDeleteAsync(GetRedisKey(queue));
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public long GetLength(string? queue = null)
//    {
//        try
//        {
//            return _db.ListLength(GetRedisKey(queue));
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> GetLengthAsync(string? queue = null)
//    {
//        try
//        {
//            return _db.ListLengthAsync(GetRedisKey(queue));
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public long Delete(T message, long count = 0, string? queue = null)
//    {
//        try
//        {
//            return _db.ListRemove(GetRedisKey(queue), _serializer.Serialize(message), count);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> DeleteAsync(T message, long count = 0, string? queue = null)
//    {
//        try
//        {
//            return _db.ListRemoveAsync(GetRedisKey(queue), _serializer.Serialize(message), count);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public long GetPosition(T message, long rank = 1, long maxLength = 0, string? queue = null)
//    {
//        try
//        {
//            return _db.ListPosition(GetRedisKey(queue), _serializer.Serialize(message), rank, maxLength);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> GetPositionAsync(T message, long rank = 1, long maxLength = 0, string? queue = null)
//    {
//        try
//        {
//            return _db.ListPositionAsync(GetRedisKey(queue), _serializer.Serialize(message), rank, maxLength);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    #region IPublisher

//    public long Publish(T message, string? key = null)
//    {
//        var redisKey = GetQueueKey(key, out var leftPush);
//        var redisValue = _serializer.Serialize(message);
//        try
//        {
//            return leftPush ? _db.ListLeftPush(redisKey, redisValue) : _db.ListRightPush(redisKey, redisValue);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public long Publish(T[] messages, string? key = null)
//    {
//        if (messages == null) throw new ArgumentNullException(nameof(messages));

//        var redisKey = GetQueueKey(key, out var leftPush);
//        var redisValues = new RedisValue[messages.Length];
//        var serializer = _serializer;
//        for (int i = 0; i < redisValues.Length; i++)
//        {
//            redisValues[i] = serializer.Serialize(messages[i]);
//        }
//        try
//        {
//            return leftPush ? _db.ListLeftPush(redisKey, redisValues) : _db.ListRightPush(redisKey, redisValues);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> PublishAsync(T message, string? key = null)
//    {
//        var redisKey = GetQueueKey(key, out var leftPush);
//        var redisValue = _serializer.Serialize(message);
//        try
//        {
//            return leftPush ? _db.ListLeftPushAsync(redisKey, redisValue) : _db.ListRightPushAsync(redisKey, redisValue);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    public Task<long> PublishAsync(T[] messages, string? key = null)
//    {
//        if (messages == null) throw new ArgumentNullException(nameof(messages));

//        var redisKey = GetQueueKey(key, out var leftPush);
//        var redisValues = new RedisValue[messages.Length];
//        var serializer = _serializer;
//        for (int i = 0; i < redisValues.Length; i++)
//        {
//            redisValues[i] = serializer.Serialize(messages[i]);
//        }
//        try
//        {
//            return leftPush ? _db.ListLeftPushAsync(redisKey, redisValues) : _db.ListRightPushAsync(redisKey, redisValues);
//        }
//        catch (RedisException ex)
//        {
//            throw new MessagingException(null, ex);
//        }
//    }

//    #endregion IPublisher

//    #region ISubscriber

//    public void Subscribe(Action<T, string?> handler, string? key = null) => _subscriber.Subscribe(handler, key);

//    public void Subscribe(Action<T[], string?> handler, string? key = null) => _subscriber.Subscribe(handler, key);

//    public Task SubscribeAsync(Action<T, string?> handler, string? key = null) => _subscriber.SubscribeAsync(handler, key);

//    public Task SubscribeAsync(Action<T[], string?> handler, string? key = null) => _subscriber.SubscribeAsync(handler, key);

//    #endregion ISubscriber

//    #endregion Public Methods

//    #region Protected Methods

//    protected RedisKey GetQueueKey(string? key, out bool leftPush)
//    {
//        leftPush = true;
//        return key ?? "default";
//    }

//    protected RedisKey GetRedisKey(string? queue)
//    {
//        return queue ?? "default";
//    }

//    #endregion Protected Methods
//}