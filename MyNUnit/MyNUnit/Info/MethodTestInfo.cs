namespace MyNUnit.Info;

using System.Reflection;
using SDK.Attributes;

public class MethodTestInfo
{
    public string Name { get; }
    
    public bool IsSuccessful { get; }

    public TimeSpan completionTime;
    
    public string? Ignored { get; }
    
    public bool HasCaughtException { get; }
    
    public ExceptionInfo? ExceptionInfo { get; }

    public MethodTestInfo(string name)
        => Name = name;

    public MethodTestInfo(string name, string ignored)
        : this (name)
        => Ignored = ignored;

    public MethodTestInfo(string name, bool isSuccessful)
        : this (name)
        => IsSuccessful = isSuccessful;

    public MethodTestInfo(string name, Type? expectedException, Exception actualException)
        : this (name)
    {
        HasCaughtException = true;

        ExceptionInfo = new ExceptionInfo(expectedException, actualException.InnerException);
        IsSuccessful = ExceptionInfo.AreExceptionsSame();
    }

    public static MethodTestInfo StartTest(object? instance, MethodBase method, TestAttribute testAttribute)
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

    public void Print()
    {
        Console.WriteLine($"Method name: {Name}");
        if (Ignored != null)
        {
            Console.WriteLine($"Ignored. Reason: {Ignored}");
            Console.WriteLine();
            return;
        }
        
        Console.WriteLine($"Is successful: {IsSuccessful}");

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