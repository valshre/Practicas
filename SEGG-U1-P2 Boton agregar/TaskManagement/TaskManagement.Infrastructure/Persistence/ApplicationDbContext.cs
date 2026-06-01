using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaskManagement.Domain.Entities;
namespace TaskManagement.Infrastructure.Persistence;
using TaskManagement.Domain;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    base(options)
    { }
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>().HasKey(t => t.Id);
        base.OnModelCreating(modelBuilder);
    }
}