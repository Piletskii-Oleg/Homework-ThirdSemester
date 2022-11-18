namespace MyFTP;

using System.Net;
using System.Net.Sockets;

public class Client
{
    private readonly IPAddress uri;
    private readonly int port;

    public Client(IPAddress uri, int port)
    {
        this.uri = uri;
        this.port = port;
    }

    public Client(string uri, int port)
    {
        this.uri = IPAddress.Parse(uri);
        this.port = port;
    }

    public async Task<string?> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(uri, port);
        await using var stream = client.GetStream();

        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync($"1 {path}");
        await writer.FlushAsync();

        using var reader = new StreamReader(stream);
        var result = await reader.ReadToEndAsync();
        if (result == "-1")
        {
            throw new DirectoryNotFoundException();
        }

        return result;
    }

    public async Task Get(string path, string newPath)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(uri, port);
        await using var stream = client.GetStream();

        await using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync($"2 {path}");
        await writer.FlushAsync();

        var byteLength = new byte[8];
        await stream.ReadAsync(byteLength);
        var length = BitConverter.ToInt64(byteLength);
        if (length == -1)
        {
            throw new FileNotFoundException();
        }

        await using var localWriter = new FileStream(newPath, FileMode.Create);
        await stream.CopyToAsync(localWriter);
    }
}