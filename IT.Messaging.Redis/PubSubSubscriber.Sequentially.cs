//using System;
//using System.Threading.Tasks;

//namespace IT.Messaging.Redis;

//public class SequentiallyPubSubSubscriber : PubSubSubscriber, ISubscriber
//{
//    public SequentiallyPubSubSubscriber(
//        StackExchange.Redis.ISubscriber subscriber,
//        IRedisValueDeserializer deserializer) : base(subscriber, deserializer)
//    {

//    }

//    public async Task SubscribeAsync(AsyncMemoryHandler? handler, AsyncMemoryArrayHandler? arrayHandler = null, string? channel = null)
//    {
//        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => arrayHandler(new[] { (ReadOnlyMemory<byte>)message.Message }, message.Channel));
//    }

//    public async Task SubscribeAsync<T>(AsyncHandler<T>? handler, AsyncArrayHandler<T>? arrayHandler = null, string? channel = null)
//    {
//        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(new[] { _deserializer.Deserialize<T>(message.Message) }, message.Channel));
//    }

//    public void Subscribe(MemoryHandler? handler, MemoryArrayHandler? arrayHandler = null, string? channel = null)
//    {
//        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => arrayHandler(new[] { (ReadOnlyMemory<byte>)message.Message }, message.Channel));
//    }

//    public void Subscribe<T>(Handler<T>? handler, ArrayHandler<T>? arrayHandler = null, string? channel = null)
//    {
//        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(new[] { _deserializer.Deserialize<T>(message.Message) }, message.Channel));
//    }
//}