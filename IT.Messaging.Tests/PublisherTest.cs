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

        _channel.Subscribe<Guid>("POIB-*", (channel, guid) =>
        {
            Console.WriteLine($"[{channel}] -> {guid}");
        });

        _channel.Publish("POIB-hash", guid);

        Assert.Pass();
    }
}