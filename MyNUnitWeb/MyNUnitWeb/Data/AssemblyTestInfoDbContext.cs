namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

public class AssemblyTestInfoDbContext : DbContext
{
    public AssemblyTestInfoDbContext(DbContextOptions<AssemblyTestInfoDbContext> options)
        : base(options)
    {
    }
}