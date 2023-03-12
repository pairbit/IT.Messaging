using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

using Internal;

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

    public long CleanAll(IEnumerable<string>? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = ToRedisKeys(queues);

        try
        {
            return _db.KeyDelete(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> CleanAllAsync(IEnumerable<string>? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = ToRedisKeys(queues);

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

    public bool Rename(string newQueue, string? queue = null)
    {
        try
        {
            return _db.KeyRename(GetRedisKey(queue), GetRedisKey(newQueue), When.NotExists);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<bool> RenameAsync(string newQueue, string? queue = null)
    {
        try
        {
            return _db.KeyRenameAsync(GetRedisKey(queue), GetRedisKey(newQueue), When.NotExists);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public long MoveAll(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null)
    {
        try
        {
            return _db.ListMoveAll(GetRedisKey(sourceQueue), GetRedisKey(destinationQueue), (ListSide)(int)sourceSide, (ListSide)(int)destinationSide);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> MoveAllAsync(string destinationQueue, Side destinationSide = Side.Left, Side sourceSide = Side.Right, string? sourceQueue = null)
    {
        try
        {
            return _db.ListMoveAllAsync(GetRedisKey(sourceQueue), GetRedisKey(destinationQueue), (ListSide)(int)sourceSide, (ListSide)(int)destinationSide);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public void Trim(long min, long max, string? queue = null)
    {
        try
        {
            _db.ListTrim(GetRedisKey(queue), min, max);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task TrimAsync(long min, long max, string? queue = null)
    {
        try
        {
            return _db.ListTrimAsync(GetRedisKey(queue), min, max);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    //public ReadOnlyMemory<byte> Move(string sourceQueue, string destinationQueue, QueueSide sourceSide, QueueSide destinationSide)
    //{
    //    try
    //    {
    //        return _db.ListMove(GetRedisKey(sourceQueue), GetRedisKey(destinationQueue), (ListSide)(int)sourceSide, (ListSide)(int)destinationSide);
    //    }
    //    catch (RedisException ex)
    //    {
    //        throw new MessagingException(null, ex);
    //    }
    //}

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

    public long Delete(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null)
    {
        var redisValues = messages.ToRedisValues(1);
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

    public Task<long> DeleteAsync(IEnumerable<ReadOnlyMemory<byte>> messages, long count = 0, string? queue = null)
    {
        var redisValues = messages.ToRedisValues(1);
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

    public long ExistsAll(IEnumerable<string>? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = ToRedisKeys(queues);

        try
        {
            return _db.KeyExists(keys);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public Task<long> ExistsAllAsync(IEnumerable<string>? queues = null)
    {
        if (queues == null) throw new ArgumentNullException(nameof(queues));

        var keys = ToRedisKeys(queues);

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

    public long[] GetPositions(ReadOnlyMemory<byte> message, long count, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPositions(GetRedisKey(queue), message, count, rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public ReadOnlyMemory<byte> GetByIndex(long index, string? queue = null)
    {
        try
        {
            return _db.ListGetByIndex(GetRedisKey(queue), index);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public ReadOnlyMemory<byte>[] GetRange(long min = 0, long max = -1, string? queue = null)
    {
        try
        {
            var redisValues = _db.ListRange(GetRedisKey(queue), min, max);
            var messages = new ReadOnlyMemory<byte>[redisValues.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = redisValues[i];
            }
            return messages;
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

    public Task<long[]> GetPositionsAsync(ReadOnlyMemory<byte> message, long count, long rank = 1, long maxLength = 0, string? queue = null)
    {
        try
        {
            return _db.ListPositionsAsync(GetRedisKey(queue), message, count, rank, maxLength);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public async Task<ReadOnlyMemory<byte>> GetByIndexAsync(long index, string? queue = null)
    {
        try
        {
            return await _db.ListGetByIndexAsync(GetRedisKey(queue), index).ConfigureAwait(false);
        }
        catch (RedisException ex)
        {
            throw new MessagingException(null, ex);
        }
    }

    public async Task<ReadOnlyMemory<byte>[]> GetRangeAsync(long min = 0, long max = -1, string? queue = null)
    {
        try
        {
            var redisValues = await _db.ListRangeAsync(GetRedisKey(queue), min, max).ConfigureAwait(false);
            var messages = new ReadOnlyMemory<byte>[redisValues.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = redisValues[i];
            }
            return messages;
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
        var redisValues = messages.ToRedisValues();
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
        var redisValues = messages.ToRedisValues();
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

    private RedisKey[] ToRedisKeys(IEnumerable<string> keys)
    {
        RedisKey[] redisKeys;

        if (keys is IReadOnlyList<string> readOnlyList)
        {
            redisKeys = new RedisKey[readOnlyList.Count];

            for (int i = 0; i < redisKeys.Length; i++)
            {
                redisKeys[i] = GetRedisKey(readOnlyList[i]);
            }
        }
        else if (keys is IList<string> list)
        {
            redisKeys = new RedisKey[list.Count];

            for (int i = 0; i < redisKeys.Length; i++)
            {
                redisKeys[i] = GetRedisKey(list[i]);
            }
        }
        else if (keys.TryGetNonEnumeratedCount(out var count))
        {
            redisKeys = new RedisKey[count];

            var i = 0;

            foreach (var key in keys)
            {
                redisKeys[i++] = key;
            }
        }
        else
        {
            var redisKeyList = new List<RedisKey>();
            foreach (var key in keys)
            {
                redisKeyList.Add(key);
            }
            redisKeys = new RedisKey[redisKeyList.Count];
            redisKeyList.CopyTo(redisKeys, 0);
        }

        return redisKeys;
    }
}