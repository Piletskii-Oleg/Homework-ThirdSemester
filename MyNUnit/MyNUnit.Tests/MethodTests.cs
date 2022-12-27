namespace MyNUnit.Tests;

using System.Reflection.Metadata;
using Info;
using State;
using TestFiles;

public class MethodTests
{
    [TestCase("ShouldPass", TestState.Passed)]
    [TestCase("HasArguments", TestState.IncorrectNumberOfParameters)]
    [TestCase("InvalidReturnType", TestState.IncorrectReturnType)]
    [TestCase("Ignored", TestState.Ignored)]
    [TestCase("ShouldFail", TestState.Failed)]
    public void MethodTestStateShouldBeCorrect(string methodName, TestState testState)
    {
        var instance = Activator.CreateInstance(typeof(MethodTestsClass));
        var method = instance?.GetType().GetMethod(methodName);

        var info = MethodTestInfo.StartTest(instance, method);
        
        Assert.That(info.State, Is.EqualTo(testState));
    }

    [Test]
    public void TestShouldPassIfExpectedExceptionIsCaught()
    {
        var instance = Activator.CreateInstance(typeof(MethodTestsClass));
        var method = instance?.GetType().GetMethod("ShouldPassWithException");
        
        var info = MethodTestInfo.StartTest(instance, method);
        Assert.Multiple(() =>
        {
            Assert.That(info.State, Is.EqualTo(TestState.Passed));
            Assert.That(info.HasCaughtException, Is.True);
            Assert.That(info.ExceptionInfo, Is.Not.Null);
        });
        
        Assert.That(info.ExceptionInfo?.ActualException, Is.Not.Null);
        Assert.That(info.ExceptionInfo?.ActualException?.GetType(), Is.EqualTo(typeof(ArgumentException)));
    }

    [Test]
    public void TestShouldNotPassIfExceptionIsExpectedButNotThrown()
    {
        var instance = Activator.CreateInstance(typeof(MethodTestsClass));
        var method = instance?.GetType().GetMethod("ShouldFailBecauseNoException");
        
        var info = MethodTestInfo.StartTest(instance, method);
        Assert.That(info.State, Is.EqualTo(TestState.Failed));
        Assert.That(info.HasCaughtException, Is.True);
        Assert.That(info.ExceptionInfo, Is.Not.Null);

        Assert.That(info.ExceptionInfo?.ActualException, Is.Null);
    }
}