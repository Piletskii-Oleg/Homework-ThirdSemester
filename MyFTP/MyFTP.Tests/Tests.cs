namespace MyFTP.Tests;

using System.Net;

public class Tests
{
    private readonly Server server = new (Path.Combine("..", "..", "..", "TestFiles"), 8888);
    private readonly Client client = new (IPAddress.Parse("127.0.0.1"), 8888);

    [OneTimeSetUp]
    public void OneTimeSetup()
        => Task.Run(async () => await server.Start());

    [Test]
    public async Task ListShouldWorkProperly()
    {
        var list = await client.List("");

        var path1 = Path.Combine("..", "..", "..", "TestFiles", "dog.jpg");
        var path2 = Path.Combine("..", "..", "..", "TestFiles", "TextFile.txt");
        var path3 = Path.Combine("..", "..", "..", "TestFiles", "Folder");

        var expected = $"{3} {path1} {false} {path2} {false} {path3} {true}";
        Assert.That(list, Is.EqualTo(expected));
    }

    [TestCase("dog.jpg")]
    [TestCase("TextFile.txt")]
    public async Task GetShouldWorkProperly(string fileName)
    {
        var originalPath = Path.Combine(@"..", "..", "..", "TestFiles", fileName);
        var pathToCopy = Path.Combine(@"..", "..", "..", "TestFiles", "Folder", fileName);

        await client.Get(fileName, pathToCopy);
        Assert.That(File.ReadAllBytes(originalPath), Is.EqualTo(File.ReadAllBytes(pathToCopy)));
    }

    [Test]
    public void ListShouldThrowExceptionIfNoFileIsFound()
    {
        var path = Path.Combine(@"..", "..", "..", "TestFiles", "NotAFolder");
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List(path));
    }

    [Test]
    public void GetShouldThrowExceptionIfNoDirectoryIsFound()
    {
        var path = Path.Combine(@"..", "..", "..", "TestFiles", "sh3.exe");
        Assert.ThrowsAsync<FileNotFoundException>(async () => await client.Get(path, ""));
    }
}