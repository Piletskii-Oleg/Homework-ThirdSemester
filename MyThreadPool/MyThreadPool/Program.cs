// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
TaskFactory taskFactory = new TaskFactory();
Task<int> task = new(() => 2);