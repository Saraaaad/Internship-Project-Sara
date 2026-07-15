using Microsoft.EntityFrameworkCore;

namespace InternshipProjectSara.Data.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<Logs> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().OwnsOne(u => u.Salary);
        base.OnModelCreating(modelBuilder);
    }
}