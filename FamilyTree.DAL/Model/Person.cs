namespace FamilyTree.DAL.Model
{
    public class Person : IEntity
    {
        public int Id { get; init; } // Уникальный идентификатор
        public string Name { get; set; } = null!;// ФИО
        public DateTime DateOfBirth { get; set; } //= new(1900, 1, 1); // Дата рождения
        public Gender Gender { get; set; } // Пол

        public List<FamilyRelation>? Parents { get; set; } // Родители
        public List<FamilyRelation>? Children { get; set; } // Дети

        public int? SpouseId { get; set; } // Идентификатор супруга (может быть null)
        public Person? Spouse { get; set; } // Навигационное свойство для супруга
        public override string ToString() => $"{Name}, {DateOfBirth}, {Enum.GetName(Gender)} ";
    }
}
