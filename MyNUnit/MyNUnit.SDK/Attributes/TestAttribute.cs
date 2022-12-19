namespace MyNUnit.SDK.Attributes;

/// <summary>
///     bruh
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; set; }

    public string? Ignored { get; set; }
}