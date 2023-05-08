namespace NetworkChat;

using System.Net.Sockets;

public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamWriter writer;

    private CancellationTokenSource tokenSource;
    
    public bool IsConnectionClosed { get; private set; }

    public Client(string uri, int port)
    {
        client = new TcpClient(uri, port);
        stream = client.GetStream();
        writer = new StreamWriter(stream);
        
        tokenSource = new();
    }

    public async Task Send(string message)
    {
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();

        if (message == "exit" || tokenSource.IsCancellationRequested)
        {
            Close();
        }
    }

    public async Task<string> Receive()
    {
        var reader = new StreamReader(stream);
        
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

    public void Close()
    {
        client.Close();
        stream.Close();
        writer.Close();

        tokenSource.Cancel();

        IsConnectionClosed = true;
    }
}