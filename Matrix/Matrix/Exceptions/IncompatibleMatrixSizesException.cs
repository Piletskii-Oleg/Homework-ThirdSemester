namespace Matrix.Exceptions;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="IncompatibleMatrixSizesException"/> class.
    /// </summary>
    /// <param name="message">Message.</param>
    public IncompatibleMatrixSizesException(string message)
        : base(message)
    {
    }
}
