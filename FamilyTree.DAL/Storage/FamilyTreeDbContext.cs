using FamilyTree.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL.Storage;

public class FamilyTreeDbContext(DbContextOptions<FamilyTreeDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; init; }
    public DbSet<FamilyRelation> FamilyRelations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FamilyRelation>()
            .HasKey(pc => new { pc.ParentId, pc.ChildId });

        modelBuilder.Entity<FamilyRelation>()
            .HasOne(pc => pc.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(pc => pc.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FamilyRelation>()
            .HasOne(pc => pc.Child)
            .WithMany(p => p.Parents)
            .HasForeignKey(pc => pc.ChildId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Spouse)
            .WithOne()
            .HasForeignKey<Person>(p => p.SpouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
