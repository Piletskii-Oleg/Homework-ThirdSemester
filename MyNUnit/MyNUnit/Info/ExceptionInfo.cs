namespace MyNUnit.Info;

public class ExceptionInfo
{
    public ExceptionInfo(Type? expectedExceptionType, Exception? actualException)
    {
        ExpectedExceptionType = expectedExceptionType;
        ActualException = actualException;
    }

    public Type? ExpectedExceptionType { get; }
    
    public Exception? ActualException { get; }

    public bool AreExceptionsSame()
    {
        if (ActualException == null)
        {
            return false;
        }
        
        return ExpectedExceptionType == ActualException.GetType();
    }
}