namespace MyNUnit.Tests.TestFiles;

using SDK.Attributes;

public class MethodTestsClass
{
    [Test]
    public void ShouldPass()
    {
        var a = 0;
        for (int i = 0; i < 100000; i++)
        {
            a += i;
        }
    }

    [Test]
    public void HasArguments(int number)
    {
        var newNumber = number + 5;
        Console.WriteLine(newNumber);
    }

    [Test]
    public int InvalidReturnType()
    {
        return 18;
    }

    [Test(Ignored = "ignore this")]
    public void Ignored()
    {
        throw new Exception();
    }

    [Test]
    public void ShouldFail()
    {
        throw new ArithmeticException();
    }

    [Test(Expected = typeof(ArgumentException))]
    public void ShouldPassWithException()
    {
        throw new ArgumentException();
    }
}