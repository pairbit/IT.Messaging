namespace IT.Messaging;

public interface IQueueTrimmer : IAsyncQueueTrimmer
{
    void Trim(long min, long max, string? queue = null);
}