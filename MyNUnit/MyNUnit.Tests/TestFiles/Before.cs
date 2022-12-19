namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class Before
{
    [Before]
    public void Method()
    {
        throw new Exception();
    }

    [Test]
    public void Test()
    {
    }
}