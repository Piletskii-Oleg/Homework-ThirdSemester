namespace MyNUnit.State;

public enum TestState
{
    Passed,
    Failed,
    Ignored,
    
    IncorrectNumberOfParameters,
    IncorrectReturnType
}