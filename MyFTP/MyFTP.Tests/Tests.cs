namespace MyFTP.Tests;

public class Tests
{
    private Server server;
    private Client client;

    [SetUp]
    public async Task Setup()
    {
        server = new Server("../../../TestFiles/", 8888);
        await server.Start();
        client = new Client("localhost", 8888);
    }

    [Test]
    public async Task ListShouldWorkProperly()
    {
        var list = await client.List("");
        var expected = File.ReadAllText("../../../TestFiles/TextFile.txt");
        Assert.That(list, Is.EqualTo(expected));
    }

    [TestCase("dog.jpg")]
    [TestCase("TextFile.txt")]
    public async Task GetShouldWorkProperly(string fileName)
    {
        var originalPath = Path.Combine("../../../TestFiles/", fileName);
        var pathToCopy = Path.Combine("../../../TestFiles/Folder/", fileName);
        await client.Get(originalPath, pathToCopy);
        Assert.That(File.ReadAllBytes(originalPath), Is.EqualTo(File.ReadAllBytes(pathToCopy)));
    }
}