namespace MyNUnitWeb.Data;

using MyNUnit.Info;

public class TestInfo
{
    public int TestInfoId { get; set; }
    
    public DateTime TestDate { get; set; }
    
    public List<AssemblyTestInfo> AssembliesTestInfo { get; set; }
}