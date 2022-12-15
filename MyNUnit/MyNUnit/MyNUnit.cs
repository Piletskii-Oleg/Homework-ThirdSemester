namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Info;
using SDK.Attributes;

public static class MyNUnit
{
    public static List<AssemblyTestInfo> StartAllTests(string path)
    {
        var info = new DirectoryInfo(path);
        var dllFiles = from file in info.GetFiles()
            where file.Extension == ".dll"
            select file;

        var assembliesInfo = new ConcurrentBag<AssemblyTestInfo>();
        Parallel.ForEach(dllFiles, file =>
        {
            var assembly = Assembly.LoadFrom(file.FullName);
            assembliesInfo.Add(StartAssemblyTests(assembly));
        });

        return assembliesInfo.ToList();
    }

    private static AssemblyTestInfo StartAssemblyTests(Assembly assembly)
    {
        var suitableTypes = GetTypes(assembly);
        var classesInfo = new ConcurrentBag<ClassTestInfo>();
        Parallel.ForEach(suitableTypes, type =>
        {
            if (type.IsAbstract)
            {
                throw new InvalidOperationException("Type cannot be abstract");
            }
            if (type.FullName == null)
            {
                throw new InvalidOperationException("Type name cannot be null");
            }

            var instance = assembly.CreateInstance(type.FullName);
            classesInfo.Add(ClassTestInfo.StartTests(type, instance));
        });

        return new AssemblyTestInfo(assembly.GetName(), classesInfo.ToList());
    }

    private static IEnumerable<Type> GetTypes(Assembly assembly) 
        => (from type in assembly.DefinedTypes
            from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == typeof(TestAttribute)
            select type).Distinct();
}