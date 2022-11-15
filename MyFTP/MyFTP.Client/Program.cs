using MyFTP;

var client = new Client("localhost", 8888);
Console.WriteLine(await client.List(""));