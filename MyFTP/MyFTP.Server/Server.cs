namespace MyFTP;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private readonly string serverPath;
    private readonly int port;

    private readonly CancellationTokenSource tokenSource = new ();
    private readonly List<Task> commands = new ();

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

    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        while (!tokenSource.IsCancellationRequested)
        {
            var socket = await listener.AcceptSocketAsync();
            commands.Add(Task.Run(async () => await Query(socket)));
        }

        Task.WaitAll(commands.ToArray());
    }

    public void Stop()
        => tokenSource.Cancel();

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
            await List(inputSplit[1], stream);
        }
        else if (inputSplit[0] == "2")
        {
            await Get(inputSplit[1], stream);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    private async Task List(string path, NetworkStream stream)
    {
        using var writer = new StreamWriter(stream);

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

            using var reader = new FileStream(path, FileMode.Open);

            await reader.CopyToAsync(stream);
        }
        catch (FileNotFoundException)
        {
            await stream.WriteAsync(BitConverter.GetBytes((long)-1));
            await stream.FlushAsync();
        }
        finally
        {
            stream.Socket.Close();
            await stream.DisposeAsync();
        }
    }
}