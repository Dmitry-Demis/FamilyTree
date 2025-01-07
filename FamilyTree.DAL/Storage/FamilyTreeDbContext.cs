using FamilyTree.DAL.Model;
using FamilyTree.DAL.Storage.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.DAL.Storage;

public class FamilyTreeDbContext(DbContextOptions<FamilyTreeDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; init; }
    public DbSet<FamilyRelation> FamilyRelations { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Регистрация конфигураций
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
        modelBuilder.ApplyConfiguration(new FamilyRelationConfiguration());
    }
}

