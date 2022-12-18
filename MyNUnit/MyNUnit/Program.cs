var path = @"C:\Users\Oleg\source\repos\TestProject\TestProject\bin\Debug\net6.0\";
var list = MyNUnit.MyNUnit.StartAllTests(path);
Console.WriteLine();

foreach (var info in list)
{
    info.Print();
}