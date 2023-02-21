﻿namespace IT.Messaging;

public interface IPublisher<T> : IAsyncPublisher<T>
{
    long Publish(T message, string? channel = null);

    long Publish(T[] messages, string? channel = null);
}