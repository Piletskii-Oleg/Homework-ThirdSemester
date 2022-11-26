namespace MyNUnit.SDK.Attributes;

/// <summary>
/// bruh
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class TestAttribute : Attribute
{
    public Type? Expected { get; private set; }

    public string? Ignored { get; private set; }

    public TestAttribute()
    {
    }

    public TestAttribute(Type expected)
        => Expected = expected;

    public TestAttribute(string ignored)
        => Ignored = ignored;
}