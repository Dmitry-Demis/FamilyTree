namespace FamilyTree.DAL.Model
{
    public class Person : IEntity
    {
        public int Id { get; init; } // Уникальный идентификатор
        public string FullName { get; set; } // ФИО
        public DateTime DateOfBirth { get; set; } // Дата рождения
        public Gender Gender { get; set; } // Пол

        public ICollection<FamilyRelation> Parents { get; set; } // Родители
        public ICollection<FamilyRelation> Children { get; set; } // Дети

        public int? SpouseId { get; set; } // Идентификатор супруга (может быть null)
        public Person Spouse { get; set; } // Навигационное свойство для супруга
    }
}
