namespace Matrix;

[Serializable]
public class IncompatibleMatrixSizesException : Exception
{
    public IncompatibleMatrixSizesException() { }
    public IncompatibleMatrixSizesException(string message) : base(message) { }
    public IncompatibleMatrixSizesException(string message, Exception inner) : base(message, inner) { }
    protected IncompatibleMatrixSizesException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
