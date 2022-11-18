using MyFTP;

if (args.Length != 2)
{
    Console.WriteLine("First parameter: Path to the server folder");
    Console.WriteLine("Second parameter: Port");
    return;
}

if (int.TryParse(args[1], out int port))
{
    if (port <= 0 || port >= 65536)
    {
        Console.WriteLine("Invalid port.");
        return;
    }

    var server = new Server(args[0], port);
    Task.Run(async () => await server.Start());
    Console.WriteLine("Server started.");

    Console.WriteLine("Write /stop to stop the server.");
    bool isStopped = false;

    while (!isStopped)
    {
        if (Console.ReadLine() == "/stop")
        {
            isStopped = true;
        }
    }

    server.Stop();
    Console.WriteLine("Server stopped.");
}
        