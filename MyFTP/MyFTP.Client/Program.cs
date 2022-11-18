using MyFTP;
using System.Net;

if (args.Length != 5 && args.Length != 4)
{
    Console.WriteLine("First parameter: IP");
    Console.WriteLine("Second parameter: Port");
    Console.WriteLine("Third parameter: -get or -list");
    Console.WriteLine("Fourth parameter: Path to the file or directory");
    Console.WriteLine("Fifth parameter (if -get is selected): path to new file");
    return;
}

if (int.TryParse(args[1], out int port))
{
    if (port <= 0 || port >= 65536)
    {
        Console.WriteLine("Invalid port.");
        return;
    }

    var client = new Client(IPAddress.Parse(args[0]), port);

    if (args[2] == "-get")
    {
        await client.Get(args[3], args[4]);
        Console.WriteLine($"Copied file to {Path.GetFullPath(args[4])}");
    }
    else if (args[2] == "-list")
    {
        Console.WriteLine(await client.List(args[3]));
    }
    else
    {
        Console.WriteLine("Incorrect command. Supported: -get and -list.");
    }
}

