namespace MyNUnit;

using System.Collections.Concurrent;
using System.Reflection;
using Info;
using SDK.Attributes;

public static class MyNUnit
{
    public static List<AssemblyTestInfo> StartTests(string path)
    {
        var info = new DirectoryInfo(path);
        var dllFiles = from file in info.GetFiles()
            where file.Extension == ".dll"
            select file;

        var assembliesInfo = new BlockingCollection<AssemblyTestInfo>();
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
        var classesInfo = new BlockingCollection<ClassTestInfo>();
        Parallel.ForEach(suitableTypes, type =>
        {
            var instance = assembly.CreateInstance(type.FullName);
            classesInfo.Add(StartOneClass(type, instance));
        });

        return new AssemblyTestInfo(assembly.GetName(), classesInfo.ToList());
    }

    private static ClassTestInfo StartOneClass(Type type, object instance)
    {
        var testMethods = GetMethods(type, typeof(TestAttribute));
        var afterMethods = GetMethods(type, typeof(AfterAttribute));
        var beforeMethods = GetMethods(type, typeof(BeforeAttribute));
        var afterClassMethods = GetMethods(type, typeof(AfterClassAttribute)).Where(method => method.IsStatic);
        var beforeClassMethods = GetMethods(type, typeof(BeforeClassAttribute)).Where(method => method.IsStatic);

        Parallel.ForEach(beforeClassMethods, method => method.Invoke(null, null));

        var methodsInfo = new List<MethodTestInfo>();
        foreach (var method in testMethods)
        {
            var testAttribute = GetTestAttribute(method);
            if (testAttribute.Ignored != null)
            {
                methodsInfo.Add(new MethodTestInfo(method.Name, testAttribute.Ignored));
                continue;
            }

            var beforeMethodsInfo = beforeMethods as MethodInfo[] ?? beforeMethods.ToArray();
            Parallel.ForEach(beforeMethodsInfo, beforeMethod => beforeMethod.Invoke(instance, null));
            
            methodsInfo.Add(StartMethod(instance, method, testAttribute));
            
            var afterMethodsInfo = afterMethods as MethodInfo[] ?? afterMethods.ToArray();
            Parallel.ForEach(afterMethodsInfo, afterMethod => afterMethod.Invoke(instance, null));
        }

        Parallel.ForEach(afterClassMethods, method => method.Invoke(null, null));

        return new ClassTestInfo(type.Name, methodsInfo);
    }

    private static MethodTestInfo StartMethod(object instance, MethodBase method, TestAttribute testAttribute)
    {
        try
        {
            method.Invoke(instance, null);
        }
        catch (Exception exception)
        {
            return new MethodTestInfo(method.Name, testAttribute.Expected, exception);
        }
        
        return new MethodTestInfo(method.Name, true);
    }

    private static TestAttribute GetTestAttribute(MethodInfo method)
    {
        var testAttributes = method.GetCustomAttributes<TestAttribute>();
        var attributes = testAttributes as TestAttribute[] ?? testAttributes.ToArray();
        
        var testAttribute = attributes[0];
        return testAttribute;
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