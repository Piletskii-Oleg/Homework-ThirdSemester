namespace MyFTP;

using System.Net;
using System.Net.Sockets;

public class Client
{
    public static async Task<string> List(string uri, int port, string path)
    {
        using var client = new TcpClient(uri, port);
        var stream = client.GetStream();

        var writer = new StreamWriter(stream);
        await writer.WriteLineAsync("List " + path);
        await writer.FlushAsync();

        var reader = new StreamReader(stream);
        var data = await reader.ReadLineAsync();
        return data;
    }

    public static async Task<string> Get(string uri, int port, string path)
    {
        using var client = new TcpClient(uri, port);

        var stream = client.GetStream();

        var writer = new StreamWriter(stream);
        writer.Write("Get " + path);
        writer.Flush();

        var reader = new StreamReader(stream);
        var data = await reader.ReadToEndAsync();
        return data;
    }
}