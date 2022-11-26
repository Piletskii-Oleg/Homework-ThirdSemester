using System.Reflection;
using MyNUnit.SDK.Attributes;

namespace MyNUnit;

public static class MyNUnit
{
    public static void StartTests(string path)
    {
        var info = new DirectoryInfo(path);
        var dllFiles = from file in info.GetFiles()
            where file.Extension == ".dll"
            select file;

        Parallel.ForEach(dllFiles, file =>
        {
            var assembly = Assembly.LoadFrom(file.FullName);
            StartAssemblyTests(assembly);
        });
    }

    private static void StartAssemblyTests(Assembly assembly)
    {
        var suitableTypes = GetTypes(assembly);
        Parallel.ForEach(suitableTypes, type =>
        {
            var instance = assembly.CreateInstance(type.FullName);
            StartOneClass(type, instance);
        });
    }
    
    private static void StartOneClass(Type type, object instance)
    {
        var testMethods = GetMethods(type, typeof(TestAttribute));
        var afterMethods = GetMethods(type, typeof(AfterAttribute));
        var beforeMethods = GetMethods(type, typeof(BeforeAttribute));
        var afterClassMethods = GetMethods(type, typeof(AfterClassAttribute)).Where(method => method.IsStatic);
        var beforeClassMethods = GetMethods(type, typeof(BeforeClassAttribute)).Where(method => method.IsStatic);
        
        Parallel.ForEach(beforeClassMethods, method => method.Invoke(null, null));
        
        foreach (var method in testMethods)
        {
            Parallel.ForEach(beforeMethods, before => before.Invoke(instance, null));
            method.Invoke(instance, null);
            Parallel.ForEach(afterMethods, after => after.Invoke(instance, null));
        }

        Parallel.ForEach(afterClassMethods, method => method.Invoke(null, null));
    }

    private static IEnumerable<MethodInfo> GetMethods(Type type, Type attributeType)
        => from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == attributeType
            select method;
    
    private static IEnumerable<Type> GetTypes(Assembly assembly)
        => (from type in assembly.DefinedTypes
            from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == typeof(TestAttribute)
            select type).Distinct();
}