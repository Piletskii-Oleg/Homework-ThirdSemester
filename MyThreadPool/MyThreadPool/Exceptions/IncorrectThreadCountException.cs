namespace MyThreadPool.Exceptions;

[Serializable]
public class IncorrectThreadCountException : Exception
{
    public IncorrectThreadCountException() { }
    public IncorrectThreadCountException(string message) : base(message) { }
    public IncorrectThreadCountException(string message, Exception inner) : base(message, inner) { }
    protected IncorrectThreadCountException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}