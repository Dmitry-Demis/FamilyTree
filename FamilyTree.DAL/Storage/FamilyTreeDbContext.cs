using FamilyTree.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL.Storage;

public class FamilyTreeDbContext(DbContextOptions<FamilyTreeDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; init; }
    public DbSet<FamilyRelation> FamilyRelations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка FamilyRelation
        modelBuilder.Entity<FamilyRelation>()
            .HasKey(fr => fr.Id); // Указываем Id как первичный ключ

        modelBuilder.Entity<FamilyRelation>()
            .HasOne(fr => fr.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(fr => fr.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FamilyRelation>()
            .HasOne(fr => fr.Child)
            .WithMany(p => p.Parents)
            .HasForeignKey(fr => fr.ChildId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка Person
        modelBuilder.Entity<Person>()
            .HasOne(p => p.Spouse)
            .WithOne()
            .HasForeignKey<Person>(p => p.SpouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

