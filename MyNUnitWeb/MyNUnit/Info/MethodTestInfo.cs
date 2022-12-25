namespace MyNUnit.Info;

using System.Diagnostics;
using System.Reflection;
using SDK.Attributes;
using State;

/// <summary>
///     Contains information about a single test.
/// </summary>
public class MethodTestInfo
{
    public MethodTestInfo()
    {
    }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="MethodTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    private MethodTestInfo(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MethodTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="ignored">Reason for ignoring a method.</param>
    private MethodTestInfo(string name, string ignored)
        : this(name)
    {
        Ignored = ignored;
        State = TestState.Ignored;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MethodTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="testState">State of the method.</param>
    private MethodTestInfo(string name, TestState testState)
        : this(name)
    {
        State = testState;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MethodTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="testState">State of the method.</param>
    /// <param name="time">Time which was required for the test to pass.</param>
    private MethodTestInfo(string name, TestState testState, TimeSpan time)
        : this(name)
    {
        State = testState;
        CompletionTime = time;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MethodTestInfo" /> class.
    /// </summary>
    /// <param name="name">Name of the method.</param>
    /// <param name="expectedExceptionType">Type of an exception that was expected.</param>
    /// <param name="actualException">The exception that was caught.</param>
    /// <param name="time">Time which was required for the test to pass.</param>
    private MethodTestInfo(string name, Type? expectedExceptionType, Exception? actualException, TimeSpan time)
        : this(name)
    {
        HasCaughtException = true;
        ExceptionInfo = new ExceptionInfo(expectedExceptionType, actualException);

        State = ExceptionInfo.AreExceptionsSame() ? TestState.Passed : TestState.Failed;
        CompletionTime = time;
    }

    public int MethodTestInfoId { get; set; }
    
    /// <summary>
    ///     Gets name of the test.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets state of the test.
    /// </summary>
    public TestState State { get; set; }

    /// <summary>
    ///     Gets time which was required to complete the test. 0 if it is ignored.
    /// </summary>
    public TimeSpan CompletionTime { get ; set; }

    /// <summary>
    ///     Gets the reason for ignoring a test if it has <see cref="TestAttribute" /> with Ignored property.
    /// </summary>
    public string? Ignored { get; set; }

    /// <summary>
    ///     Gets a value indicating whether an exception was caught or not.
    /// </summary>
    public bool HasCaughtException { get; set; }

    /// <summary>
    ///     Gets information about an exception if it was caught.
    /// </summary>
    public ExceptionInfo? ExceptionInfo { get; set; }

    /// <summary>
    ///     Starts the test and returns information about it.
    /// </summary>
    /// <param name="instance">Instance on which the test is executed.</param>
    /// <param name="method">Method that should be executed.</param>
    /// <returns>Information about the test.</returns>
    public static MethodTestInfo StartTest(object? instance, MethodInfo method)
    {
        var testAttribute = GetTestAttribute(method);

        if (testAttribute.Ignored != null) return new MethodTestInfo(method.Name, testAttribute.Ignored);

        if (method.GetParameters().Length != 0)
            return new MethodTestInfo(method.Name, TestState.IncorrectNumberOfParameters);

        if (method.ReturnType != typeof(void)) return new MethodTestInfo(method.Name, TestState.IncorrectReturnType);

        var stopwatch = new Stopwatch();

        try
        {
            stopwatch.Start();
            method.Invoke(instance, null);
        }
        catch (TargetInvocationException exception)
        {
            stopwatch.Stop();

            return new MethodTestInfo(method.Name, testAttribute.Expected, exception.InnerException, stopwatch.Elapsed);
        }

        stopwatch.Stop();

        return new MethodTestInfo(method.Name, TestState.Passed, stopwatch.Elapsed);
    }

    private static TestAttribute GetTestAttribute(MemberInfo method)
    {
        var testAttributes = method.GetCustomAttributes<TestAttribute>();
        var attributes = testAttributes as TestAttribute[] ?? testAttributes.ToArray();

        var testAttribute = attributes[0];

        return testAttribute;
    }
}