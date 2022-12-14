using NetworkChat;

string endString = "exit";

if (args.Length == 1)
{
    var server = new Server(int.Parse(args[0]));
    await server.Start();
    Console.WriteLine("Server opened");
    
    var sendTask = Task.Run(async () =>
    {
        while (!server.IsClosed)
        {
            var message = Console.ReadLine();
            await server.Send(message);
        }
    });

    var receiveTask = Task.Run(async () =>
    {
        while (!server.IsClosed)
        {
            Console.WriteLine(await server.Receive());
        }
    });

    Task.WaitAny(sendTask, receiveTask);
}
else if (args.Length == 2)
{
    var client = new Client(args[0], int.Parse(args[1]));
    Console.WriteLine("Client opened");
    
    var sendTask = Task.Run(async () =>
    {
        while (!client.IsConnectionClosed)
        {
            var message = Console.ReadLine();
            await client.Send(message);
        }
    });
    
    var receiveTask = Task.Run(async () =>
    {
        while (!client.IsConnectionClosed)
        {
            Console.WriteLine(await client.Receive());
        }
    });

    Task.WaitAny(sendTask, receiveTask);
}