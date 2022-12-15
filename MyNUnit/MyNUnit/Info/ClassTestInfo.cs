namespace MyNUnit.Info;

using System.Reflection;
using SDK.Attributes;

public class ClassTestInfo
{
    public string Name { get; }
    
    public IReadOnlyList<MethodTestInfo> MethodsInfo { get; }

    private ClassTestInfo(string name, IReadOnlyList<MethodTestInfo> methodsInfo)
    {
        Name = name;
        MethodsInfo = methodsInfo;
    }
    
    public static ClassTestInfo StartTests(Type type, object instance)
    {
        StartSupplementaryClassMethods(type, typeof(BeforeClassAttribute));
        
        var testMethods = GetMethods(type, typeof(TestAttribute));
        var methodsInfo = new List<MethodTestInfo>();
        foreach (var method in testMethods)
        {
            var testAttribute = GetTestAttribute(method);

            StartSupplementaryMethods(type, instance, typeof(BeforeAttribute));

            methodsInfo.Add(method.IsStatic
                ? MethodTestInfo.StartTest(null, method, testAttribute)
                : MethodTestInfo.StartTest(instance, method, testAttribute));

            StartSupplementaryMethods(type, instance, typeof(AfterAttribute));
        }

        StartSupplementaryClassMethods(type, typeof(AfterClassAttribute));
        
        return new ClassTestInfo(type.Name, methodsInfo);
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

    private static void StartSupplementaryMethods(Type type, object instance, Type attributeType)
    {
        var methods = GetMethods(type, attributeType);
        var methodsInfo = methods as MethodInfo[] ?? methods.ToArray();
        
        Parallel.ForEach(methodsInfo, method => method.Invoke(instance, null));
    }

    private static void StartSupplementaryClassMethods(Type type, Type attributeType)
    {
        var classMethods = GetMethods(type, attributeType);
        var classMethodsInfo = classMethods as MethodInfo[] ?? classMethods.ToArray();

        if (classMethodsInfo.Any(method => !method.IsStatic))
        {
            throw new ArgumentException($"Methods with BeforeClassAttribute or AfterClassAttribute must be static", nameof(type));
        }

        Parallel.ForEach(classMethodsInfo, method => method.Invoke(null, null));
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
}