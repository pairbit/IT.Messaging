using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class SequentiallyPubSubChannel : PubSubChannel, IChannel
{
    public SequentiallyPubSubChannel(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueSerializer serializer,
        IRedisValueDeserializer deserializer) : base(subscriber, serializer, deserializer)
    {

    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(message.Message, message.Channel));
    }

    public async Task SubscribeAsync<T>(Action<T, string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(_deserializer.Deserialize<T>(message.Message), message.Channel));
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(message.Message, message.Channel));
    }

    public void Subscribe<T>(Action<T, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(_deserializer.Deserialize<T>(message.Message), message.Channel));
    }
}