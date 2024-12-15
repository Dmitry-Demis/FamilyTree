using FamilyTree.DAL.Model;
using FamilyTree.DAL.Storage;

namespace FamilyTree.BLL.Services;

public class FamilyService(IRepository<Person> personRepository) : IFamilyTreeService
{
    private readonly IRepository<Person> _personRepository = personRepository
                                                             ?? throw new ArgumentNullException(nameof(personRepository));
    public async Task<bool> CreatePersonAsync(Person person)
    {
        // Проверка обязательных полей
        ArgumentNullException.ThrowIfNull(person.FullName, "Полное имя не может быть пустым.");
        ArgumentNullException.ThrowIfNull(person.DateOfBirth, "Дата рождения не может быть пустой.");
        ArgumentNullException.ThrowIfNull(person.Gender, "Пол не может быть пустым.");


        try
        {
            // Добавление нового человека в базу данных
            await _personRepository.AddAsync(person);

            return true;
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            throw new InvalidOperationException("Ошибка при создании человека", ex);
        }
    }

    public async Task<IEnumerable<Person>> LoadFamilyTreeAsync()
    {
        //try
        //{
        //    // Загружаем все данные о людях
        //    var allPeople = await _personRepository.GetAllAsync();

        //    // Строим дерево, где каждый человек будет содержать список детей
        //    foreach (var person in allPeople)
        //    {
        //        // Загружаем детей для каждого человека
        //        person.Children = allPeople.Where(p => p.ParentId == person.Id).ToList();
        //    }

        //    // Возвращаем список людей с построенными связями
        //    return allPeople.Where(p => p.ParentId == null);  // Возвращаем корневых людей (у которых нет родителей)
        //}
        //catch (Exception ex)
        //{
        //    throw new InvalidOperationException("Ошибка при загрузке семейного дерева", ex);
        //}
        return [];
    }
}