//using StackExchange.Redis;
//using System;
//using System.Threading.Tasks;

//namespace IT.Messaging.Redis;

//public class ConcurrentlyPubSubSubscriber : PubSubSubscriber, ISubscriber
//{
//    public ConcurrentlyPubSubSubscriber(
//        StackExchange.Redis.ISubscriber subscriber,
//        IRedisValueDeserializer deserializer) : base(subscriber, deserializer)
//    {

//    }

//    public async Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryArrayHandler? arrayHandler = null, string? channel = null)
//    {
//        RedisChannel redisChannel = channel == null ? ChannelDefault : channel;
//        await _subscriber.SubscribeAsync(redisChannel, (rChannel, rValue) => arrayHandler(new[] { (ReadOnlyMemory<byte>)rValue }, rChannel)).ConfigureAwait(false);
//    }

//    public async Task SubscribeAsync<T>(AsyncHandler<T> handler, AsyncArrayHandler<T>? arrayHandler = null, string? channel = null)
//    {
//        await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { _deserializer.Deserialize<T>(rValue) }, rChannel)).ConfigureAwait(false);
//    }

//    public void Subscribe(MemoryHandler? handler, MemoryArrayHandler? arrayHandler = null, string? channel = null)
//    {
//        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => arrayHandler(new[] { (ReadOnlyMemory<byte>)rValue }, rChannel));
//    }

//    public void Subscribe<T>(Action<T[], string?> handler, string? channel = null)
//    {
//        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { _deserializer.Deserialize<T>(rValue) }, rChannel));
//    }
//}