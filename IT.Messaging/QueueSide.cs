namespace IT.Messaging;

public readonly record struct QueueSide
{
    private readonly string _queue;
    private readonly Side _side;

    public string Queue => _queue;

    public Side Side => _side;

    public QueueSide(string queue, Side side)
    {
        _queue = queue;
        _side = side;
    }
}