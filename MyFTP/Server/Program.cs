using MyFTP;
var path = @"C:\Users\Oleg\Documents\GitHub\courses\programming-3rd-semester";
Console.WriteLine($"Запускаем сервер в папке {path}...");
var server = new Server(path, 8888);
await server.Start();