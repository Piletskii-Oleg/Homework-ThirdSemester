namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class ClassTestsClass
{
    public int Number { get; private set; }
    
    [BeforeClass]
    public static void BeforeClassMethod()
    {
    }

    [Test]
    public void Test()
    {
        int number = Number;
    }
}