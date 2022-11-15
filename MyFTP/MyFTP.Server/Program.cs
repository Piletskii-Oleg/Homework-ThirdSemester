using MyFTP;

var path = "../../../TestFiles/";
Console.WriteLine($"Запускаем сервер в папке {path}...");
var server = new Server(path, 8888);
await server.Start();