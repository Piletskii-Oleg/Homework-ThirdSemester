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

    public async Task Query(Socket socket)
    {
        Console.WriteLine("Подключен клиент.");
        var stream = new NetworkStream(socket);

        Console.WriteLine("Клиент соединен.");
        var reader = new StreamReader(stream);

        Console.WriteLine("Чтение файлов...");
        var input = await reader.ReadLineAsync();

        Console.WriteLine("Чтение окончено.");
        var inputSplit = input.Split();

        if (inputSplit[0] == "List")
        {
            await List(inputSplit[1], stream);
            socket.Close();
        }
        else if (inputSplit[0] == "Get")
        {
            await Get(inputSplit[1]);
            socket.Close();
        }
        else
        {
            socket.Close();
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

    private async Task<string> Get(string path)
    {
        return "";
    }
}