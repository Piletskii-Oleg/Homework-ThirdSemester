namespace MyNUnit.Info;

public class MethodTestInfo
{
    public string Name { get; }
    
    public bool IsSuccessful { get; }

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
}