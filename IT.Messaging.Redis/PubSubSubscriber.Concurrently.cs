﻿using System;
using System.Threading.Tasks;

namespace IT.Messaging.Redis;

public class ConcurrentlyPubSubSubscriber : PubSubSubscriber, ISubscriber
{
    public ConcurrentlyPubSubSubscriber(
        StackExchange.Redis.ISubscriber subscriber,
        IRedisValueDeserializer deserializer) : base(subscriber, deserializer)
    {

    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(rValue, rChannel)).ConfigureAwait(false);
    }

    public async Task SubscribeAsync(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { (ReadOnlyMemory<byte>)rValue }, rChannel)).ConfigureAwait(false);
    }

    public async Task SubscribeAsync<T>(Action<T, string?> handler, string? channel = null)
    {
        await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(_deserializer.Deserialize<T>(rValue), rChannel)).ConfigureAwait(false);
    }

    public async Task SubscribeAsync<T>(Action<T[], string?> handler, string? channel = null)
    {
        await _subscriber.SubscribeAsync(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { _deserializer.Deserialize<T>(rValue) }, rChannel)).ConfigureAwait(false);
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(rValue, rChannel));
    }

    public void Subscribe(Action<ReadOnlyMemory<byte>[], string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { (ReadOnlyMemory<byte>)rValue }, rChannel));
    }

    public void Subscribe<T>(Action<T, string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(_deserializer.Deserialize<T>(rValue), rChannel));
    }

    public void Subscribe<T>(Action<T[], string?> handler, string? channel = null)
    {
        _subscriber.Subscribe(channel == null ? ChannelDefault : channel, (rChannel, rValue) => handler(new[] { _deserializer.Deserialize<T>(rValue) }, rChannel));
    }
}