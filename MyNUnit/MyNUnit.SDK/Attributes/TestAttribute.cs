namespace MyNUnit.SDK.Attributes;

/// <summary>
///     bruh
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    public TestAttribute()
    {
    }

    public TestAttribute(Type expected)
    {
        Expected = expected;
    }

    public TestAttribute(string ignored)
    {
        Ignored = ignored;
    }

    public Type? Expected { get; }

    public string? Ignored { get; }
}