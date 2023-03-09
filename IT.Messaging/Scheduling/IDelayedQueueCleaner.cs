namespace IT.Messaging.Scheduling;

public interface IDelayedQueueCleaner : IAsyncDelayedQueueCleaner, IQueueCleaner
{
    long CleanRange(long? minDelay, long? maxDelay, string? queue = null);
}