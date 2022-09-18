namespace Matrix;

/// <summary>
/// Exception that gets thrown when an attempt to multiply matrices with incompatible sizes is made.
/// </summary>
[Serializable]
public class IncompatibleMatrixSizesException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncompatibleMatrixSizesException"/> class.
    /// </summary>
    public IncompatibleMatrixSizesException()
    {
    }
}
