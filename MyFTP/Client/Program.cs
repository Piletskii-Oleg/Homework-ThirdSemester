using System.Threading.Channels;
using MyFTP;

var a = await Client.List("localhost", 8888, "05-networks-1");
Console.WriteLine(a);