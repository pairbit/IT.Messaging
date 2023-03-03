using System;

namespace IT.Messaging.Scheduling;

public readonly struct MessageDelay
{
    private readonly long _delay;
    private readonly ReadOnlyMemory<byte> _message;

    public long Delay => _delay;

    public ReadOnlyMemory<byte> Message => _message;

    public MessageDelay(long delay, ReadOnlyMemory<byte> message)
    {
        _delay = delay;
        _message = message;
    }
}