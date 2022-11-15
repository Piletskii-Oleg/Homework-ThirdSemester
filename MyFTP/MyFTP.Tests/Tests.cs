namespace MyFTP.Tests;

public class Tests
{
    private Server server;
    private Client client;
    [SetUp]
    public void Setup()
    {
        server = new Server("../../../TestFiles/", 8888);
        client = new Client("localhost", 8888);
    }

    [Test]
    public async Task ListWorksProperly()
    {
        var list = await client.List("");
        Assert.That(list, Is.EqualTo(""));
    }
}