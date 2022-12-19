namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class After
{
    [After]
    public void Method()
    {
        throw new Exception();
    }

    [Test]
    public void Test()
    {
    }
}