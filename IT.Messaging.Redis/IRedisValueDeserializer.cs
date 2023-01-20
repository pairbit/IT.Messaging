using StackExchange.Redis;

namespace IT.Messaging.Redis;

public interface IRedisValueDeserializer
{
    T Deserialize<T>(in RedisValue redisValue);
}