namespace MyNUnit.Info;

public class ExceptionInfo
{
    public ExceptionInfo(Type? expectedException, Exception? actualException)
    {
        ExpectedException = expectedException;
        ActualException = actualException;
    }

    public Type? ExpectedException { get; }
    
    public Exception? ActualException { get; }

    public bool AreExceptionsSame()
        => ExpectedException == ActualException.InnerException.GetType();
}