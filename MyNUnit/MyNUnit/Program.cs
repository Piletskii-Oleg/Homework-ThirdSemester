

/*var assemblies = AppDomain.CurrentDomain.GetAssemblies();
foreach (var ass in assemblies)
{
    foreach (var type in ass.DefinedTypes)
    {
        foreach (var info in type.GetMethods())
        {
            foreach (var attribute in Attribute.GetCustomAttributes(info))
            {
                if (attribute.GetType() == typeof(TestAttribute))
                {
                    Console.WriteLine($"{info.Name}, {attribute}");
                }
            }
        }
    }
}*/

var path = @"C:\Users\Oleg\source\repos\TestProject\TestProject\bin\Debug\net6.0\";
var list = MyNUnit.MyNUnit.StartAllTests(path);
Console.WriteLine();

foreach (var info in list)
{
    info.Print();
}

// var assembly = Assembly.LoadFrom(path);
// foreach (var type in assembly.DefinedTypes)
// {
//     foreach (var method in type.GetMethods())
//         Console.WriteLine(method.Name);
// }