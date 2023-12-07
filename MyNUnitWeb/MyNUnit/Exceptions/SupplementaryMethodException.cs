namespace MyNUnit.Exceptions;

using global::MyNUnit.SDK.Attributes;
using global::MyNUnit.State;

/// <summary>
///     Exception that is thrown if an exception occured in a supplementary method
///     with <see cref="BeforeAttribute" /> or <see cref="AfterAttribute" />.
/// </summary>
public class SupplementaryMethodException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SupplementaryMethodException" /> class.
    /// </summary>
    /// <param name="classState">State of class according to method where an exception was thrown.</param>
    public SupplementaryMethodException(ClassState classState)
    {
        ClassState = classState;
    }

    /// <summary>
    ///     Gets state of the class.
    /// </summary>
    public ClassState ClassState { get; }
}