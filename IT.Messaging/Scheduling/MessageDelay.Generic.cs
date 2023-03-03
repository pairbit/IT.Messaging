namespace IT.Messaging.Scheduling;

public readonly struct MessageDelay<T>
{
    private readonly long _delay;
    private readonly T _message;

    public long Delay => _delay;

    public T Message => _message;

    public MessageDelay(long delay, T message)
    {
        _delay = delay;
        _message = message;
    }
}