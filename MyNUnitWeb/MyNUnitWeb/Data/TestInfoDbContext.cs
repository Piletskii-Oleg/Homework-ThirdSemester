namespace MyNUnitWeb.Data;

using Microsoft.EntityFrameworkCore;

public class TestInfoDbContext : DbContext
{
    public TestInfoDbContext(DbContextOptions<TestInfoDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestInfo> TestsInfo => Set<TestInfo>();
}