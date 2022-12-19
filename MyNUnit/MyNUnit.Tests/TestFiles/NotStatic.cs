namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class NotStatic
{
    [AfterClass]
    public void Method()
    {
    }

    [Test]
    public void Test()
    {
    }
}