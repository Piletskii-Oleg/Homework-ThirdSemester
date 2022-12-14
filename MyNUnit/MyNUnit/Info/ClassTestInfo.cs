namespace MyNUnit.Info;

public class ClassTestInfo
{
    public string Name { get; }
    
    public IReadOnlyList<MethodTestInfo> MethodsInfo { get; }

    public ClassTestInfo(string name, IReadOnlyList<MethodTestInfo> methodsInfo)
    {
        Name = name;
        MethodsInfo = methodsInfo;
    }

    public void Print()
    {
        Console.WriteLine($"Class {Name}");

        foreach (var methodInfo in MethodsInfo)
        {
            methodInfo.Print();
        }

        Console.WriteLine();
    }
}