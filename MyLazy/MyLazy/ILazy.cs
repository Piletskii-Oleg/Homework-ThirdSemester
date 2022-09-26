namespace MyLazy;

/// <summary>
/// Represents an object whose value is calculated only when it is called.
/// </summary>
/// <typeparam name="T">Variable type.</typeparam>
public interface ILazy<out T>
{
    /// <summary>
    /// Calculates value only once when it is called.
    /// </summary>
    /// <returns>Calculated value.</returns>
    T Get();
}
