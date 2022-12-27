namespace MyNUnitWeb.Data;

using MyNUnit.Info;
using MyNUnit.State;

public class AssemblyTestInfoDb
{
    public int AssemblyTestInfoDbId { get; set; }
    
    /// <summary>
    ///     Gets name of the assembly.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     Gets list of <see cref="ClassTestInfo" /> of the assembly.
    /// </summary>
    public List<ClassTestInfoDb> ClassesInfo { get; set; }
    
    public int GetSuccessfulTestsCount()
    {
        return ClassesInfo
            .SelectMany(classInfo => classInfo.MethodsInfo)
            .Sum(methodInfo => methodInfo.State == TestState.Passed ? 1 : 0);
    }

    public int GetUnsuccessfulTestsCount()
    {
        return ClassesInfo
            .SelectMany(classInfo => classInfo.MethodsInfo)
            .Sum(methodInfo => methodInfo.State != TestState.Passed ? 1 : 0);
    }
}