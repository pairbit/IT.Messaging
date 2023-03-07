namespace IT.Messaging.Redis;

public class QueueSubscriberConfig
{
    public string Queue { get; set; }

    public int BatchSize { get; set; } = 1;

    public int Tasks { get; set; } = 1;

    public int Delay { get; set; }
}