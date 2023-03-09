namespace IT.Messaging.Scheduling;

public interface IDelayedQueueInformer : IAsyncDelayedQueueInformer, IQueueInformer
{
    long GetLengthRange(long? minDelay, long? maxDelay, string? queue = null);
}