namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class AfterClass
{
    [AfterClass]
    public static void Method()
    {
        throw new Exception();
    }

    [Test]
    public void Test()
    {
    }
}