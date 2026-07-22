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
        modelBuilder.Entity<User>().OwnsOne(u => u.Salary, salary =>
        {
            salary.Property(s => s.Amount).HasColumnType("decimal(18,2)");
            salary.Property(s => s.Bonus).HasColumnType("decimal(18,2)");
            salary.Property(s => s.Currency).HasMaxLength(3);
        });

        modelBuilder.Entity<Tasks>()
           .HasOne(t => t.Employee)
           .WithMany(u => u.Tasks)
           .HasForeignKey(t => t.EmployeeId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Note>()
            .HasOne(n => n.Employee)
            .WithMany(u => u.Notes)
            .HasForeignKey(n => n.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(u => u.DepartmentId);

        modelBuilder.Entity<Logs>(entity =>
        {
            entity.ToTable("Logs");
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}