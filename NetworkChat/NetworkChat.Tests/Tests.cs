namespace NetworkChat.Tests;

public class Tests
{
    private const int port = 8888;
    
    private Server server;
    private Client client;
    
    [SetUp]
    public async Task Setup()
    {
        server = new Server(port);
        Task.Run(async () => await server.Start());
        client = new Client("localhost", port);
    }

    [TearDown]
    public void Disable()
    {
        server.Close();
        client.Close();
    }

    [Test]
    public async Task ServerShouldReceiveMessage()
    {
        await client.Send("i am client");
        Assert.That(await server.Receive(), Is.EqualTo("i am client"));
    }
    
    [Test]
    public async Task ClientShouldReceiveMessage()
    {
        while (!server.HasConnectionOpened)
        {
            Thread.Sleep(100);
        }
        
        await server.Send("i am client");
        Assert.That(await client.Receive(), Is.EqualTo("i am client"));
    }

    [Test]
    public async Task ServerAndClientShouldCloseSimultaneously()
    {
        while (!server.HasConnectionOpened)
        {
            Thread.Sleep(100);
        }
        
        await client.Send("exit");
        Thread.Sleep(1000);
        await server.Receive();
        
        Assert.Multiple(() =>
        {
            Assert.That(client.IsConnectionClosed, Is.True);
            Assert.That(server.IsClosed, Is.True);
        });
    }
}