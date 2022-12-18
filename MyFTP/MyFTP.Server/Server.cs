namespace MyFTP.Server;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server implementation of the FTP protocol.
/// </summary>
public class Server
{
    private readonly string serverPath;
    private readonly int port;

    private readonly CancellationTokenSource tokenSource = new();
    private readonly List<Task> commands = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="path">Path to the folder where server will operate.</param>
    /// <param name="port">Port that should be watched.</param>
    /// <exception cref="DirectoryNotFoundException">Throws if the specified <paramref name="path"/> does not exist.</exception>
    public Server(string path, int port)
    {
        if (Directory.Exists(path))
        {
            this.serverPath = path;
        }
        else
        {
            throw new DirectoryNotFoundException();
        }

        this.port = port;
    }

    /// <summary>
    /// Starts the server at the specified folder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();

        while (!this.tokenSource.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            this.commands.Add(Task.Run(async () => await this.Query(socket)));
        }

        Task.WaitAll(this.commands.ToArray());
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
        => this.tokenSource.Cancel();

    private async Task Query(Socket socket)
    {
        var stream = new NetworkStream(socket);
        using var reader = new StreamReader(stream);

        var input = await reader.ReadLineAsync();
        if (input == null)
        {
            throw new InvalidOperationException();
        }

        var inputSplit = input.Split();
        if (inputSplit[0] == "1")
        {
            await this.List(inputSplit[1], stream);
        }
        else if (inputSplit[0] == "2")
        {
            await this.Get(inputSplit[1], stream);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private async Task List(string path, NetworkStream stream)
    {
        await using var writer = new StreamWriter(stream);

        path = Path.Combine(this.serverPath, path);

        try
        {
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);

            var fileCount = files.Length + directories.Length;
            await writer.WriteAsync($"{fileCount}");

            foreach (var file in files)
            {
                await writer.WriteAsync($" {file} {false}");
                await writer.FlushAsync();
            }

            foreach (var directory in directories)
            {
                await writer.WriteAsync($" {directory} {true}");
                await writer.FlushAsync();
            }

            await writer.FlushAsync();
        }
        catch (DirectoryNotFoundException)
        {
            await writer.WriteAsync("-1");
            await writer.FlushAsync();
        }
        finally
        {
            stream.Socket.Close();
            await stream.DisposeAsync();
        }
    }

    private async Task Get(string path, NetworkStream stream)
    {
        await using var writer = new StreamWriter(stream);
        path = Path.Combine(this.serverPath, path);
        var info = new FileInfo(path);

        try
        {
            await stream.WriteAsync(BitConverter.GetBytes(info.Length));

            await using var reader = new FileStream(path, FileMode.Open);

            await reader.CopyToAsync(stream);
        }
        catch (FileNotFoundException)
        {
            await stream.WriteAsync(BitConverter.GetBytes(-1L));
            await stream.FlushAsync();
        }
        finally
        {
            stream.Socket.Close();
            await stream.DisposeAsync();
        }
    }
}