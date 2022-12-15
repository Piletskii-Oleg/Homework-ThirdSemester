namespace MyNUnit.Info;

using System.Reflection;
using SDK.Attributes;

public class MethodTestInfo
{
    public string Name { get; }
    
    public TestState State { get; }

    public TimeSpan completionTime;
    
    public string? Ignored { get; }
    
    public bool HasCaughtException { get; }
    
    public ExceptionInfo? ExceptionInfo { get; }

    private MethodTestInfo(string name)
        => Name = name;

    private MethodTestInfo(string name, string ignored)
        : this (name)
    {
        Ignored = ignored;
        State = TestState.Ignored;
    }

    private MethodTestInfo(string name, TestState testState)
        : this (name)
        => State = testState;

    private MethodTestInfo(string name, Type? expectedException, Exception actualException)
        : this (name)
    {
        HasCaughtException = true;
        ExceptionInfo = new ExceptionInfo(expectedException, actualException.InnerException);
        
        State = ExceptionInfo.AreExceptionsSame() ? TestState.Passed : TestState.Failed;
    }

    public static MethodTestInfo StartTest(object? instance, MethodInfo method, TestAttribute testAttribute)
    {
        if (testAttribute.Ignored != null)
        {
            return new MethodTestInfo(method.Name, testAttribute.Ignored);
        }

        if (method.GetParameters().Length != 0 || method.ReturnType != typeof(void))
        {
            throw new ArgumentException("Method should have no parameters and be of type void.", nameof(method));
        }

        try
        {
            method.Invoke(instance, null);
        }
        catch (TargetInvocationException exception)
        {
            return new MethodTestInfo(method.Name, testAttribute.Expected, exception);
        }
        
        return new MethodTestInfo(method.Name, TestState.Passed);
    }

    public void Print()
    {
        Console.WriteLine($"Method name: {Name}");
        Console.WriteLine($"Test State: {State}");
        
        if (Ignored != null)
        {
            Console.WriteLine($"Ignore reason: {Ignored}");
            Console.WriteLine();
            return;
        }

        if (HasCaughtException)
        {
            if (ExceptionInfo == null)
            {
                throw new InvalidOperationException();
            }
            
            Console.WriteLine($"Has caught exception {ExceptionInfo.ActualException}");

            Console.WriteLine(ExceptionInfo.ExpectedException == null
                ? "Exception that was expected: none"
                : $"Exception that was expected: {ExceptionInfo.ExpectedException}");
        }

        Console.WriteLine();
    }
}