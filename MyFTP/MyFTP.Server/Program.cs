using MyFTP;

var path = @"C:\Users\Oleg\Documents\GitHub\Homework-SecondYear\MyFTP\MyFTP.Tests\TestFiles";
Console.WriteLine($"Запускаем сервер в папке {path}...");
var server = new Server(path, 8888);
await server.Start();