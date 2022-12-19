namespace MyNUnit.Info;

using System.Collections.Concurrent;
using System.Reflection;
using SDK.Attributes;

public class AssemblyTestInfo
{
    public AssemblyName Name { get; }
    
    public IReadOnlyList<ClassTestInfo> ClassesInfo { get; }

    private AssemblyTestInfo(AssemblyName name, IReadOnlyList<ClassTestInfo> classesInfo)
    {
        Name = name;
        ClassesInfo = classesInfo;
    }
    
    public static AssemblyTestInfo StartAssemblyTests(Assembly assembly)
    {
        var suitableTypes = GetTypes(assembly);
        var classesInfo = new ConcurrentBag<ClassTestInfo>();
        Parallel.ForEach(suitableTypes, type =>
        {
            if (type.FullName == null)
            {
                throw new InvalidOperationException("Type name cannot be null");
            }

            var instance = assembly.CreateInstance(type.FullName);
            classesInfo.Add(ClassTestInfo.StartTests(type, instance));
        });

        return new AssemblyTestInfo(assembly.GetName(), classesInfo.ToList());
    }
    
    public void Print()
    {
        Console.WriteLine($"Assembly name: {Name}");
        foreach (var classInfo in ClassesInfo)
        {
            classInfo.Print();
        }
    }
    
    private static IEnumerable<Type> GetTypes(Assembly assembly) 
        => (from type in assembly.DefinedTypes
            from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == typeof(TestAttribute)
            select type).Distinct();
}