namespace IT.Messaging.Tests;

public class BaseTest
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Subscribe_Default()
    {
        var batchTrue = Batch.True;
        Assert.True(ReferenceEquals(batchTrue, Batch.True));

        Assert.False(ReferenceEquals(Batch.True, Batch.False));
        Assert.False(ReferenceEquals(Batch.True, new bool[0]));
        Assert.False(ReferenceEquals(Batch.True, Array.Empty<bool>()));
        Assert.False(ReferenceEquals(Batch.False, new bool[0]));
        Assert.False(ReferenceEquals(Batch.False, Array.Empty<bool>()));
    }
}