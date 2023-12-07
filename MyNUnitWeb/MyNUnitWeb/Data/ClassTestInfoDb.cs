namespace MyNUnitWeb.Data;

using MyNUnit.State;

public class ClassTestInfoDb
{
    public int ClassTestInfoDbId { get; set; }
    
    public string Name { get; set; }
    
    public List<MethodTestInfoDb>? MethodsInfo { get; set; }
    
    public ClassState State { get; set; }
}