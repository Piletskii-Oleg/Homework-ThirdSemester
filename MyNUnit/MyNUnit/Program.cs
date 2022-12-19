if (args.Length != 1)
{
    Console.WriteLine("Please give a single argument: path to the folder with assemblies that should be tested.");
    return;
}

if (!Directory.Exists(args[0]))
{
    Console.WriteLine("No directory found by such path.");
    return;
}

var list = MyNUnit.MyNUnit.StartAllTests(args[0]);

foreach (var info in list)
{
    info.Print();
    Console.WriteLine();
}