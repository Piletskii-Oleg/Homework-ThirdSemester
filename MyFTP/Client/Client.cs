namespace MyFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

public static class Client
{
    public static async Task<string?> List(string uri, int port, string path)
    {
        using var client = new TcpClient(uri, port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("List " + path);
        await writer.FlushAsync();

        var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }

    public static async Task<string> Get(string uri, int port, string path)
    {
        using var client = new TcpClient(uri, port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("Get " + path);
        await writer.FlushAsync();

        var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}