namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class BeforeClass
{
    [BeforeClass]
    public static void Method()
    {
        throw new Exception();
    }

    [Test]
    public void Test()
    {
    }
}