using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IT.Messaging.Redis.Internal;

internal static class Linq
{
    public static bool TryGetNonEnumeratedCount<T>(this IEnumerable<T> source, out int count)
    {
        if (source is IReadOnlyCollection<T> readOnlyCollection)
        {
            count = readOnlyCollection.Count;
            return true;
        }
        else if (source is ICollection<T> messageCollection)
        {
            count = messageCollection.Count;
            return true;
        }
        else if (source is ICollection collection)
        {
            count = collection.Count;
            return true;
        }
#if NET6_0_OR_GREATER
        else if (System.Linq.Enumerable.TryGetNonEnumeratedCount(source, out count)) return true;
#endif
        count = 0;
        return false;
    }

    public static RedisValue[] ToRedisValues(this IEnumerable<ReadOnlyMemory<byte>> messages, int index = 0)
    {
        RedisValue[] redisValues;

        if (messages is IReadOnlyList<ReadOnlyMemory<byte>> readOnlyList)
        {
            redisValues = new RedisValue[readOnlyList.Count + index];

            for (int i = 0; i < readOnlyList.Count; i++)
            {
                redisValues[i + index] = readOnlyList[i];
            }
        }
        else if (messages is IList<ReadOnlyMemory<byte>> listGeneric)
        {
            redisValues = new RedisValue[listGeneric.Count + index];

            for (int i = 0; i < listGeneric.Count; i++)
            {
                redisValues[i + index] = listGeneric[i];
            }
        }
        //else if (messages is System.Collections.IList list)
        //{
        //    redisValues = new RedisValue[list.Count + index];

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        redisValues[i + index] = (ReadOnlyMemory<byte>)list[i];
        //    }
        //}
        else if (messages.TryGetNonEnumeratedCount(out var count))
        {
            redisValues = new RedisValue[count + index];

            foreach (var message in messages)
            {
                redisValues[index++] = message;
            }
        }
        else
        {
            var redisValueList = new List<RedisValue>();
            foreach (var message in messages)
            {
                redisValueList.Add(message);
            }
            redisValues = new RedisValue[redisValueList.Count + index];
            redisValueList.CopyTo(redisValues, index);
        }

        return redisValues;
    }

    public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> messages, IRedisValueSerializer serializer, int index = 0)
    {
        RedisValue[] redisValues;

        if (messages is IReadOnlyList<T> readOnlyList)
        {
            redisValues = new RedisValue[readOnlyList.Count + index];

            for (int i = 0; i < readOnlyList.Count; i++)
            {
                redisValues[i + index] = serializer.Serialize(readOnlyList[i]);
            }
        }
        else if (messages is IList<T> listGeneric)
        {
            redisValues = new RedisValue[listGeneric.Count + index];

            for (int i = 0; i < listGeneric.Count; i++)
            {
                redisValues[i + index] = serializer.Serialize(listGeneric[i]);
            }
        }
        //else if (messages is System.Collections.IList list)
        //{
        //    redisValues = new RedisValue[list.Count + index];

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        redisValues[i + index] = serializer.Serialize((T)list[i]);
        //    }
        //}
        else if (messages.TryGetNonEnumeratedCount(out var count))
        {
            redisValues = new RedisValue[count + index];

            foreach (var message in messages)
            {
                redisValues[index++] = serializer.Serialize(message);
            }
        }
        else
        {
            var redisValueList = new List<RedisValue>();
            foreach (var message in messages)
            {
                redisValueList.Add(serializer.Serialize(message));
            }
            redisValues = new RedisValue[redisValueList.Count + index];
            redisValueList.CopyTo(redisValues, index);
        }

        return redisValues;
    }
}