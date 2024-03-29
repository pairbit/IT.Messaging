using IT.Messaging.Generic;

namespace IT.Messaging.Tests;

public class PublisherGenericTest
{
    private IChannel<Guid> _channel;

    [SetUp]
    public void Setup(IChannel<Guid> channel)
    {
        _channel = channel;
    }

    [Test]
    public void Test1()
    {
        var guid = Guid.NewGuid();

        _channel.Subscribe((Guid guid, string? channel, CancellationToken token) =>
        {
            Console.WriteLine(guid);
            return true;
        });

        _channel.Publish(guid);

        _channel.Subscribe((Guid guid, string? channel, CancellationToken token) =>
        {
            Console.WriteLine($"[{channel}] -> {guid}");
            return true;
        }, key: "POIB-*");

        _channel.Publish(guid, "POIB-*");

        Assert.Pass();
    }
}