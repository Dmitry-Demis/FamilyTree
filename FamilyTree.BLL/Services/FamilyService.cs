using FamilyTree.DAL.Model;
using FamilyTree.DAL.Storage;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.BLL.Services;

public class FamilyService(IRepository<Person> personRepository) : IFamilyTreeService
{
    private readonly IRepository<Person> _repository = personRepository
                                                             ?? throw new ArgumentNullException(nameof(personRepository));
    public async Task<bool> AddPersonToTreeAsync(Person person)
    {
        // Проверка обязательных полей
        ArgumentNullException.ThrowIfNull(person.Name, "Полное имя не может быть пустым.");
        ArgumentNullException.ThrowIfNull(person.DateOfBirth, "Дата рождения не может быть пустой.");
        ArgumentNullException.ThrowIfNull(person.Gender, "Пол не может быть пустым.");


        try
        {
            // Добавление нового человека в базу данных
            await _repository.AddAsync(person);

            return true;
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            throw new InvalidOperationException("Ошибка при создании человека", ex);
        }
    }


    public async Task<bool> RemovePersonAsync(int personId)
    {
        try
        {
            // Получаем человека из базы данных
            var person = await _repository.GetAsync(personId);

            // Если человек не найден, выбрасываем исключение
            if (person == null)
            {
                throw new InvalidOperationException("Человек не найден.");
            }

            // Удаляем человека из базы данных
            await _repository.RemoveAsync(personId);

            return true;
        }
        catch (Exception ex)
        {
            // Обработка ошибок
            throw new InvalidOperationException("Ошибка при удалении человека", ex);
        }
    }


    public async Task<IEnumerable<Person>> LoadPeopleAsync()
    {
        try
        {
            // Загружаем людей вместе с их связями
            var people = await _repository.Items
                .Include(p => p.Parents) // Загружаем родителей
                .ThenInclude(fr => fr.Parent) // Включаем информацию о родителях
                .Include(p => p.Children) // Загружаем детей
                .ThenInclude(fr => fr.Child) // Включаем информацию о детях
                .Include(p => p.Spouse) // Загружаем супругов
                .ToListAsync();

            return people;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка при загрузке семейного дерева", ex);
        }
    }


    public async Task<bool> AddParentChildRelationAsync(Person parent, Person child)
    {
        // Проверка, что родитель и ребёнок — не один и тот же человек
        if (parent.Id == child.Id)
        {
            return false; // Невозможно создать связь между одним и тем же человеком
        }

        // Загружаем существующие сущности из репозитория
        var existingParent = await _repository.GetAsync(parent.Id);
        var existingChild = await _repository.GetAsync(child.Id);

        // Проверяем, существуют ли родитель и ребёнок
        if (existingParent == null || existingChild == null)
        {
            return false; // Одна из сущностей не найдена
        }

        // Проверяем, существует ли уже связь
        // Проверяем, существует ли уже связь между родителем и ребёнком
        if (existingParent.Children?.Any(c => c.ChildId == child.Id) == true ||
            existingChild.Parents?.Any(p => p.ParentId == parent.Id) == true)
        {
            return false; // Связь уже существует
        }

        // Создаём новую связь
        var newRelation = new FamilyRelation
        {
            ParentId = parent.Id,
            ChildId = child.Id
        };

        // Добавляем связь в коллекции
        if (existingParent.Children == null)
        {
            existingParent.Children = new List<FamilyRelation>();
        }
        existingParent.Children.Add(newRelation);

        if (existingChild.Parents == null)
        {
            existingChild.Parents = new List<FamilyRelation>();
        }
        existingChild.Parents.Add(newRelation);

        // Сохраняем изменения через репозиторий
        await _repository.UpdateAsync(existingParent);
        await _repository.UpdateAsync(existingChild);

        return true; // Связь успешно добавлена
    }

    public async Task<bool> AddSpouseRelationAsync(Person man, Person woman)
    {
        if (man == null || woman == null)
            throw new ArgumentNullException("Мужчина или женщина не могут быть null.");

        if (man.Gender != Gender.Male || woman.Gender != Gender.Female)
            throw new ArgumentException("Мужчина должен иметь пол 'Male', а женщина — 'Female'.");

        if (man.SpouseId != null || woman.SpouseId != null)
            throw new InvalidOperationException("Один из участников уже состоит в браке.");

        try
        {
            // Установка супругов друг для друга
            man.SpouseId = woman.Id;
            woman.SpouseId = man.Id;

            man.Spouse = woman;
            woman.Spouse = man;

            // Сохранение изменений в базе данных
            await _repository.UpdateAsync(man);
            await _repository.UpdateAsync(woman);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при добавлении супружеской связи: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<Person>> GetAllDescendantsAsync(Person p)
    {
        var descendants = new List<Person>();
        var visited = new HashSet<int>();

        async Task DFSAsync(int currentId)
        {
            // Загружаем текущего человека
            var person = await _repository.GetAsync(currentId);
            if (person?.Children == null) return;

            foreach (var relation in person.Children)
            {
                if (relation.ChildId == null || !visited.Add(relation.ChildId.Value)) continue;

                // Помечаем узел как посещённый

                // Загружаем данные о ребёнке
                var child = await _repository.GetAsync(relation.ChildId.Value);
                if (child == null) continue;

                // Добавляем потомка и запускаем обход его потомков
                descendants.Add(child);
                await DFSAsync(child.Id);
            }
        }

        await DFSAsync(p.Id);
        return descendants;
    }

    public async Task<IEnumerable<Person>> GetAllAncestorsAsync(Person person)
    {
        var ancestors = new List<Person>();
        var visited = new HashSet<int>();

        async Task DFSAsync(int currentId)
        {
            // Загружаем текущего человека
            var currentPerson = await _repository.GetAsync(currentId);
            if (currentPerson?.Parents == null) return;

            foreach (var relation in currentPerson.Parents)
            {
                if (relation.ParentId == null || !visited.Add(relation.ParentId.Value)) continue;

                // Загружаем данные о родителе
                var parent = await _repository.GetAsync(relation.ParentId.Value);
                if (parent == null) continue;

                // Добавляем предка и запускаем обход его предков
                ancestors.Add(parent);
                await DFSAsync(parent.Id);
            }
        }

        await DFSAsync(person.Id);
        return ancestors;
    }

    public async Task<IEnumerable<Person>> GetCommonAncestorsAsync(Person p1, Person p2)
    {
        if (p1 == null || p2 == null)
            throw new ArgumentNullException("Оба человека должны быть указаны.");

        // Получаем всех предков первого человека
        var ancestors1 = (await GetAllAncestorsAsync(p1)).ToHashSet();

        // Получаем всех предков второго человека
        var ancestors2 = (await GetAllAncestorsAsync(p2)).ToHashSet();

        // Находим пересечение двух множеств предков
        var commonAncestors = ancestors1.Intersect(ancestors2);

        return commonAncestors;
    }
}