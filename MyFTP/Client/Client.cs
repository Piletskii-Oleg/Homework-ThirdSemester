namespace MyFTP;

using System.Net.Sockets;

public class Client
{
    private readonly string uri;
    private readonly int port;

    public Client(string uri, int port)
    {
        this.uri = uri;
        this.port = port;
    }

    public async Task<string?> List(string path)
    {
        using var client = new TcpClient(uri, port);
        await using var stream = client.GetStream();

        using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("List " + path);
        await writer.FlushAsync();

        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }

    public async Task Get(string path, string newPath)
    {
        using var client = new TcpClient(uri, port);
        await using var stream = client.GetStream();

        await using var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("Get " + path);
        await writer.FlushAsync();

        using var localWriter = new BinaryWriter(File.Create(newPath));

        var byteLength = new byte[8];
        await stream.ReadAsync(byteLength);
        var length = BitConverter.ToInt64(byteLength);

        for (int i = 0; i < length; i++)
        {
            var buffer = new byte[1];
            await stream.ReadAsync(buffer).ConfigureAwait(false);

            localWriter.Write(buffer);
        }

        localWriter.Flush();
    }
}