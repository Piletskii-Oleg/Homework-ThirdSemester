namespace MyNUnit.Info;

using System.Reflection;
using SDK.Attributes;
using State;

public class ClassTestInfo
{
    public string Name { get; }
    
    public IReadOnlyList<MethodTestInfo>? MethodsInfo { get; }

    public ClassState State { get; }

    private ClassTestInfo(string name, IReadOnlyList<MethodTestInfo> methodsInfo)
    {
        Name = name;
        MethodsInfo = methodsInfo;
        State = ClassState.Passed;
    }

    private ClassTestInfo(string name, ClassState state)
    {
        Name = name;
        State = state;
    }
    
    public static ClassTestInfo StartTests(Type type, object instance)
    {
        if (type.IsAbstract)
        {
            return new ClassTestInfo(type.Name, ClassState.ClassIsAbstract);
        }
        
        var state = StartSupplementaryClassMethods(type, typeof(BeforeClassAttribute));
        if (state != ClassState.Passed)
        {
            return new ClassTestInfo(type.Name, state);
        }

        var testMethods = GetMethods(type, typeof(TestAttribute));
        var methodsInfo = new List<MethodTestInfo>();
        foreach (var method in testMethods)
        {
            state = StartSupplementaryMethods(type, instance, typeof(BeforeAttribute));
            if (state != ClassState.Passed)
            {
                return new ClassTestInfo(type.Name, state);
            }

            methodsInfo.Add(method.IsStatic
                ? MethodTestInfo.StartTest(null, method)
                : MethodTestInfo.StartTest(instance, method));

            state = StartSupplementaryMethods(type, instance, typeof(AfterAttribute));
            if (state != ClassState.Passed)
            {
                return new ClassTestInfo(type.Name, state);
            }
        }

        state = StartSupplementaryClassMethods(type, typeof(AfterClassAttribute));
        if (state != ClassState.Passed)
        {
            return new ClassTestInfo(type.Name, state);
        }

        return new ClassTestInfo(type.Name, methodsInfo);
    }
    
    public void Print()
    {
        Console.WriteLine($"- Class {Name}");
        Console.WriteLine($"- State: {State}");

        if (MethodsInfo == null)
        {
            return;
        }
        
        foreach (var methodInfo in MethodsInfo)
        {
            methodInfo.Print();
        }
    }

    private static ClassState StartSupplementaryMethods(Type type, object instance, Type attributeType)
    {
        var methods = GetMethods(type, attributeType);
        var methodsInfo = methods as MethodInfo[] ?? methods.ToArray();

        try
        {
            Parallel.ForEach(methodsInfo, method => method.Invoke(instance, null));
        }
        catch (AggregateException exception)
        {
            if (exception.InnerException is TargetInvocationException)
            {
                if (attributeType == typeof(BeforeAttribute))
                {
                    return ClassState.BeforeMethodFailed;
                }

                if (attributeType == typeof(AfterAttribute))
                {
                    return ClassState.AfterMethodFailed;
                }
            }
        }

        return ClassState.Passed;
    }

    private static ClassState StartSupplementaryClassMethods(Type type, Type attributeType)
    {
        var classMethods = GetMethods(type, attributeType);
        var classMethodsInfo = classMethods as MethodInfo[] ?? classMethods.ToArray();

        if (classMethodsInfo.Any(method => !method.IsStatic))
        {
            return ClassState.ClassMethodWasNotStatic;
        }

        try
        {
            Parallel.ForEach(classMethodsInfo, method => method.Invoke(null, null));
        }
        catch (AggregateException exception)
        {
            if (exception.InnerException is TargetInvocationException)
            {
                if (attributeType == typeof(BeforeClassAttribute))
                {
                    return ClassState.BeforeClassMethodFailed;
                }

                if (attributeType == typeof(AfterClassAttribute))
                {
                    return ClassState.AfterClassMethodFailed;
                }
            }
        }

        return ClassState.Passed;
    }

    private static IEnumerable<MethodInfo> GetMethods(Type type, Type attributeType)
        => from method in type.GetMethods()
            from attribute in Attribute.GetCustomAttributes(method)
            where attribute.GetType() == attributeType
            select method;
}