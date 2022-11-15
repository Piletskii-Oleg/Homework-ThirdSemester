using MyFTP;

var path = @"C:\Games\Higurashi\Higurashi When They Cry Hou - Ch.3 Tatarigoroshi";
Console.WriteLine($"Запускаем сервер в папке {path}...");
var server = new Server(path, 8888);
await server.Start();