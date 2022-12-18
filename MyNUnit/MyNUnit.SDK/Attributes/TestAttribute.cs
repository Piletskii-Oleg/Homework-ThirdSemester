namespace MyNUnit.SDK.Attributes;

/// <summary>
///     bruh
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; }

    public string? Ignored { get; }
    
    public TestAttribute()
    {
    }

    public TestAttribute(Type expected)
        => Expected = expected;

    public TestAttribute(string ignored)
        => Ignored = ignored;
}