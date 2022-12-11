namespace MyNUnit;

using System.Reflection;
using SDK.Attributes;

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
            var testAttribute = GetTestAttribute(method);
            if (testAttribute.Ignored != null)
            {
                Console.WriteLine($"Method {method.Name} is ignored. Reason: {testAttribute.Ignored}.\n");
                continue;
            }

            var beforeMethodInfos = beforeMethods as MethodInfo[] ?? beforeMethods.ToArray();
            Parallel.ForEach(beforeMethodInfos, beforeMethod => beforeMethod.Invoke(instance, null));
            
            StartMethod(instance, method, testAttribute);
            
            var afterMethodInfos = afterMethods as MethodInfo[] ?? afterMethods.ToArray();
            Parallel.ForEach(afterMethodInfos, afterMethod => afterMethod.Invoke(instance, null));
        }

        Parallel.ForEach(afterClassMethods, method => method.Invoke(null, null));
    }

    private static void StartMethod(object instance, MethodBase method, TestAttribute testAttribute)
    {
        Console.WriteLine($"Starting method: {method.Name}");
        var success = false;
        var caughtException = false;
        try
        {
            method.Invoke(instance, null);
        }
        catch (Exception exception)
        {
            caughtException = true;
            if (exception.InnerException?.GetType() == testAttribute.Expected)
            {
                Console.WriteLine("Caught exception {testAttribute.Expected}.");
                success = true;
            }
            else
            {
                Console.WriteLine($"Warning: unhandled exception: {exception.InnerException}.");
                if (testAttribute.Expected != null)
                {
                    Console.WriteLine($"Exception of type {testAttribute.Expected} was expected.");
                }
            }
        }
        finally
        {
            if (!caughtException && testAttribute.Expected != null)
            {
                Console.WriteLine($"Expected exception of type {testAttribute.Expected} but got nothing.");
            }
            else if (!caughtException && testAttribute.Expected == null)
            {
                success = true;
            }
        }

        Console.WriteLine(success
            ? $"Method {method.Name} has run successfully."
            : $"Method {method.Name} has run unsuccessfully.");
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