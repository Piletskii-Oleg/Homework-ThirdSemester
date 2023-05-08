namespace NetworkChat;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private readonly int port;

    private Socket socket;

    private readonly CancellationTokenSource tokenSource;
    
    public bool HasConnectionOpened { get; private set; }
    
    public bool IsClosed { get; private set; }
    
    public Server(int port)
    {
        this.port = port;
        tokenSource = new CancellationTokenSource();
    }
    
    public async Task Start()
    {
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        
        socket = await listener.AcceptSocketAsync();
        HasConnectionOpened = true;

        listener.Stop();
    }

    public async Task<string> Receive()
    {
        await using var stream = new NetworkStream(socket);
        using var reader = new StreamReader(stream);

        var receivedMessage = await reader.ReadLineAsync();
        if (receivedMessage == null)
        {
            throw new InvalidOperationException();
        }
        
        if (receivedMessage == "exit" || tokenSource.IsCancellationRequested)
        {
            Close();
        }

        return receivedMessage;
    }

    public async Task Send(string message)
    {
        await using var stream = new NetworkStream(socket);
        await using var writer = new StreamWriter(stream);
        
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
        
        if (message == "exit" || tokenSource.IsCancellationRequested)
        {
            Close();
        }
    }

    public void Close()
    {
        socket.Close();

        tokenSource.Cancel();
        
        IsClosed = true;
    }
}