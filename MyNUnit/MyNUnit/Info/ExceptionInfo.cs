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
    {
        if (ActualException == null)
        {
            return false;
        }
        if (ExpectedException == ActualException.GetType())
        {
            return true;
        }
        
        return false;
    }
}