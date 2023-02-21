﻿using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class SequentiallyPubSubSubscriber : PubSubSubscriber, ISubscriber
{
    public SequentiallyPubSubSubscriber(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueDeserializer deserializer) : base(subscriber, deserializer)
    {

    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(message.Message, message.Channel));
    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(new[] { (ReadOnlyMemory<byte>)message.Message }, message.Channel));
    }

    public async Task SubscribeAsync<T>(Action<T, string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(_deserializer.Deserialize<T>(message.Message), message.Channel));
    }

    public async Task SubscribeAsync<T>(Action<T[], string?> handler, string? channel = null)
    {
        (await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel).ConfigureAwait(false)).OnMessage(message => handler(new[] { _deserializer.Deserialize<T>(message.Message) }, message.Channel));
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(message.Message, message.Channel));
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(new[] { (ReadOnlyMemory<byte>)message.Message }, message.Channel));
    }

    public void Subscribe<T>(Action<T, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(_deserializer.Deserialize<T>(message.Message), message.Channel));
    }

    public void Subscribe<T>(Action<T[], string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel).OnMessage(message => handler(new[] { _deserializer.Deserialize<T>(message.Message) }, message.Channel));
    }
}