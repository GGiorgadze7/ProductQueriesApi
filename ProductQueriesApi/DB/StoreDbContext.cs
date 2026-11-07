using Microsoft.EntityFrameworkCore;
using ProductQueriesApi.Entities;

namespace ProductQueriesApi.DB;

public class StoreDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=localhost\\MSSQLSERVER01;Integrated Security=True; Database=StoreDb; Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0");
    }
}
