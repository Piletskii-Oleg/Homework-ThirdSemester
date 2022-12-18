namespace MyFTP.Client;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Client implementation of the FTP protocol.
/// </summary>
public class Client
{
    private readonly IPAddress uri;
    private readonly int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="uri">Address of the server.</param>
    /// <param name="port">Port of the server.</param>
    public Client(IPAddress uri, int port)
    {
        this.uri = uri;
        this.port = port;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="uri">Address of the server.</param>
    /// <param name="port">Port of the server.</param>
    public Client(string uri, int port)
    {
        this.uri = IPAddress.Parse(uri);
        this.port = port;
    }

    /// <summary>
    /// Shows amount of files and directories and list of all of them.
    /// </summary>
    /// <param name="path">Path to the folder relative to server's path.</param>
    /// <returns>A <see cref="Task"/> with amount of files and directories and list of all of them.</returns>
    /// <exception cref="DirectoryNotFoundException">Throws if the specified directory does not exist.</exception>
    public async Task<string?> List(string path)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.uri, this.port);
        await using var stream = client.GetStream();

        await using var writer = new StreamWriter(stream);
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

    /// <summary>
    /// Gets the file on the specified path and copies it to a local path.
    /// </summary>
    /// <param name="path">Path to the required file relative to server's path.</param>
    /// <param name="newPath">Path at which file should be saved.</param>
    /// <exception cref="FileNotFoundException">Throws if the specified file does not exist.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Get(string path, string newPath)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.uri, this.port);
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