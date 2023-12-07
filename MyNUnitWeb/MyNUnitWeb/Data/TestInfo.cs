namespace MyNUnitWeb.Data;

public class TestInfo
{
    public int TestInfoId { get; set; }
    
    public DateTime TestDate { get; set; }
    
    public List<AssemblyTestInfoDb> AssembliesTestInfo { get; set; }
}