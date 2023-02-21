namespace IT.Messaging.Tests;

public class PublisherTest
{
    private IChannel _channel;

    [SetUp]
    public void Setup(IChannel channel)
    {
        _channel = channel;
    }

    [Test]
    public void Test1()
    {
        var guid = Guid.NewGuid();

        _channel.Subscribe((Guid[] guid, string? channel) =>
        {
            Console.WriteLine($"[{channel}] -> {guid}");
        }, "POIB-*");

        _channel.Publish(guid, "POIB-hash");

        Assert.Pass();
    }
}