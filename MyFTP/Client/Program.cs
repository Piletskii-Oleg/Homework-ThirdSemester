using System.Threading.Channels;
using MyFTP;

var b = await Client.Get("localhost", 8888, "EULA.txt");
Console.WriteLine(b);