namespace MyNUnit.Info;

using System.Diagnostics;
using System.Reflection;
using SDK.Attributes;

public class MethodTestInfo
{
    public string Name { get; }
    
    public TestState State { get; }

    public TimeSpan CompletionTime { get; }
    
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

    private MethodTestInfo(string name, TestState testState, TimeSpan time)
        : this (name)
    {
        State = testState;
        CompletionTime = time;
    }

    private MethodTestInfo(string name, Type? expectedException, Exception? actualException, TimeSpan time)
        : this (name)
    {
        HasCaughtException = true;
        ExceptionInfo = new ExceptionInfo(expectedException, actualException);
        
        State = ExceptionInfo.AreExceptionsSame() ? TestState.Passed : TestState.Failed;
        CompletionTime = time;
    }

    public static MethodTestInfo StartTest(object? instance, MethodInfo method)
    {
        var testAttribute = GetTestAttribute(method);
        
        if (testAttribute.Ignored != null)
        {
            return new MethodTestInfo(method.Name, testAttribute.Ignored);
        }

        if (method.GetParameters().Length != 0)
        {
            throw new ArgumentException("Method should have no parameters", nameof(method));
        }

        if (method.ReturnType != typeof(void))
        {
            throw new ArgumentException("Method should be of type void.", nameof(method));
        }

        var stopwatch = new Stopwatch();
        
        try
        {
            stopwatch.Start();
            method.Invoke(instance, null);
        }
        catch (TargetInvocationException exception)
        {
            stopwatch.Stop();
            return new MethodTestInfo(method.Name, testAttribute.Expected, exception.InnerException, stopwatch.Elapsed);
        }

        stopwatch.Stop();
        return new MethodTestInfo(method.Name, TestState.Passed, stopwatch.Elapsed);
    }

    public void Print()
    {
        Console.WriteLine($"--- Method name: {Name}");
        Console.WriteLine($"    Test State: {State}");
        
        if (Ignored != null)
        {
            Console.WriteLine($"    Ignore reason: {Ignored}");
            Console.WriteLine();
            return;
        }

        if (HasCaughtException)
        {
            if (ExceptionInfo == null)
            {
                throw new InvalidOperationException();
            }
            
            Console.WriteLine($"    Has caught exception {ExceptionInfo.ActualException}");

            Console.WriteLine(ExceptionInfo.ExpectedExceptionType == null
                ? "    Exception that was expected: none"
                : $"    Exception that was expected: {ExceptionInfo.ExpectedExceptionType}");
        }

        Console.WriteLine($"    Time required: {CompletionTime.Milliseconds} ms.");
        Console.WriteLine();
    }

    private static TestAttribute GetTestAttribute(MemberInfo method)
    {
        var testAttributes = method.GetCustomAttributes<TestAttribute>();
        var attributes = testAttributes as TestAttribute[] ?? testAttributes.ToArray();
        
        var testAttribute = attributes[0];
        return testAttribute;
    }
}