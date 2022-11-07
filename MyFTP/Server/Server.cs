namespace MyFTP;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private TcpListener listener;
    private readonly string serverPath;

    public Server(string path, int port)
    {
        this.serverPath = path;

        listener = new TcpListener(IPAddress.Any, port);
    }

    public async Task Start()
    {
        listener.Start();

        while (true)
        {
            var socket = listener.AcceptSocket();
            await Query(socket);
        }
    }

    private async Task Query(Socket socket)
    {
        var stream = new NetworkStream(socket);
        var reader = new StreamReader(stream);

        var input = await reader.ReadLineAsync();
        var inputSplit = input.Split();

        if (inputSplit[0] == "List")
        {
            await List(inputSplit[1], stream);
        }
        else if (inputSplit[0] == "Get")
        {
            await Get(inputSplit[1], stream);
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private async Task List(string path, NetworkStream stream)
    {
        await Task.Run(async () =>
        {
            var writer = new StreamWriter(stream);
            path = Path.Combine(this.serverPath, path);
            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);

            var fileCount = files.Length + directories.Length;
            await writer.WriteAsync($"{fileCount}");

            foreach (var file in files)
            {
                await writer.WriteAsync($" {file} {false}");
            }

            foreach (var directory in directories)
            {
                await writer.WriteAsync($" {directory} {true}");
            }

            await writer.WriteLineAsync();
            await writer.FlushAsync();
        });

        
    }

    private async Task Get(string path, NetworkStream stream)
    {
        await Task.Run(async () =>
        {
            using var writer = new StreamWriter(stream);
            using var binaryWriter = new BinaryWriter(stream);
            path = Path.Combine(this.serverPath, path);

            var info = new FileInfo(path);
            await writer.WriteAsync($"{info.Length} ");

            var result = File.ReadAllLines(path);
            for (int i = 0; i < result.Length; i++)
            {
                await writer.WriteLineAsync(result[i]);
            }
        });
    }
}