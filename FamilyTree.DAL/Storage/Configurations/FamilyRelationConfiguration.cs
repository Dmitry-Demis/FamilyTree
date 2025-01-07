using FamilyTree.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.DAL.Storage.Configurations;

public class FamilyRelationConfiguration : IEntityTypeConfiguration<FamilyRelation>
{
    public void Configure(EntityTypeBuilder<FamilyRelation> builder)
    {
        // Устанавливаем первичный ключ
        builder.HasKey(fr => fr.Id);

        // Настройка связи "Родитель-Ребёнок"
        builder
            .HasOne(fr => fr.Parent)
            .WithMany(p => p.Children)
            .HasForeignKey(fr => fr.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(fr => fr.Child)
            .WithMany(p => p.Parents)
            .HasForeignKey(fr => fr.ChildId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}