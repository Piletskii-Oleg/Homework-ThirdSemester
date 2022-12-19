namespace MyNUnit.State;

public enum ClassState
{
    Passed,
    
    BeforeMethodFailed,
    AfterMethodFailed,
    
    ClassMethodWasNotStatic,
    
    BeforeClassMethodFailed,
    AfterClassMethodFailed,
    
    ClassIsAbstract
}