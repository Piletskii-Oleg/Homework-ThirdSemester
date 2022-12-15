namespace MyNUnit.Info;

using System.Reflection;

public class AssemblyTestInfo
{
    public AssemblyName Name { get; }
    
    public IReadOnlyList<ClassTestInfo> ClassesInfo { get; }

    public AssemblyTestInfo(AssemblyName name, IReadOnlyList<ClassTestInfo> classesInfo)
    {
        Name = name;
        ClassesInfo = classesInfo;
    }

    public void Print()
    {
        foreach (var classInfo in ClassesInfo)
        {
            classInfo.Print();
        }

        Console.WriteLine();
    }
}