namespace MyNUnit.Tests;

using TestFiles;
using Info;
using State;

public class ClassTests
{
    [Test]
    public void CorrectClassShouldPassTests()
    {
        var type = typeof(ClassTestsClass);
        var instance = Activator.CreateInstance(type);
        var classInfo = ClassTestInfo.StartTests(type, instance);

        Assert.That(classInfo.State, Is.EqualTo(ClassState.Passed));
    }

    [TestCase(typeof(Before), ClassState.BeforeMethodFailed)]
    [TestCase(typeof(After), ClassState.AfterMethodFailed)]
    [TestCase(typeof(BeforeClass), ClassState.BeforeClassMethodFailed)]
    [TestCase(typeof(AfterClass), ClassState.AfterClassMethodFailed)]
    [TestCase(typeof(NotStatic), ClassState.ClassMethodWasNotStatic)]
    public void IncorrectClassesShouldGiveReasonForNotPassingTests(Type type, ClassState classState)
    {
        var instance = Activator.CreateInstance(type);
        var classInfo = ClassTestInfo.StartTests(type, instance);

        Assert.That(classInfo.State, Is.EqualTo(classState));
    }
}