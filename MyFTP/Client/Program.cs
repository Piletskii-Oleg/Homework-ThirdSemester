using System.Threading.Channels;
using MyFTP;

Console.WriteLine(await Client.List("localhost", 8888, ""));
var b = await Client.Get("localhost", 8888, "EULA.txt");
Console.WriteLine(b);