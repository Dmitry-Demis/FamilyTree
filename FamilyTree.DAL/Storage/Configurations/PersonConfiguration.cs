using FamilyTree.DAL.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.DAL.Storage.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Настройка связи "Супруг(а)"
            builder
                .HasOne(p => p.Spouse)
                .WithOne()
                .HasForeignKey<Person>(p => p.SpouseId)
                .OnDelete(DeleteBehavior.SetNull);

            // Дополнительные настройки для сущности Person можно добавить здесь
        }
    }
}
