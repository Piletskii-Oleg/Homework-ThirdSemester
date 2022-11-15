namespace MyFTP.Tests;

public class Tests
{
    private readonly Server server = new (@"..\..\..\TestFiles\", 8888);
    private readonly Client client = new ("localhost", 8888);

    [OneTimeSetUp]
    public void Setup()
    {
        Task.Run(async () => await server.Start());
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
        var originalPath = Path.Combine(@"..\..\..\TestFiles\", fileName);
        var pathToCopy = Path.Combine(@"..\..\..\TestFiles\Folder", fileName);

        await client.Get(fileName, pathToCopy);
        Assert.That(File.ReadAllBytes(originalPath), Is.EqualTo(File.ReadAllBytes(pathToCopy)));
    }
}