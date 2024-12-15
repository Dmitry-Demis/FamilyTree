namespace FamilyTree.DAL.Model.Builders;

public class PersonBuilder : BuilderBase<Person, PersonBuilder>
{
    // Установка полного имени
    public PersonBuilder SetFullName(string fullName) => SetProperty(nameof(Person.FullName), fullName);

    // Установка даты рождения
    public PersonBuilder SetDateOfBirth(DateTime dateOfBirth) => SetProperty(nameof(Person.DateOfBirth), dateOfBirth);

    // Установка пола
    public PersonBuilder SetGender(Gender gender) => SetProperty(nameof(Person.Gender), gender);
}