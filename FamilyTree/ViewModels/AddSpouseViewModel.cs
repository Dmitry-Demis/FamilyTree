using FamilyTree.Presentation.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MathCore.ViewModels;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using System.Windows.Input;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using MathCore.WPF.Commands;
using System.Collections.Generic;

namespace FamilyTree.Presentation.ViewModels
{
    public class AddSpouseViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        public string Title => "Добавить супругов";

        // Коллекция мужчин
        public ObservableCollection<PersonWrapper> Men { get; } = new ObservableCollection<PersonWrapper>();

        // Коллекция женщин, отфильтрованных для выбранного мужчины
        public ObservableCollection<PersonWrapper> Women { get; } = new ObservableCollection<PersonWrapper>();

        // Словарь всех людей, разделённых по полу
        private Dictionary<Gender, List<PersonWrapper>> _peopleByGender = new();

        // Выбранный мужчина
        private PersonWrapper? _selectedMan;
        public PersonWrapper? SelectedMan
        {
            get => _selectedMan;
            set
            {
                if (Set(ref _selectedMan, value))
                {
                    UpdateWomenList(); // Обновляем список женщин при изменении выбранного мужчины
                }
            }
        }

        // Выбранная женщина
        private PersonWrapper? _selectedWoman;
        public PersonWrapper? SelectedWoman
        {
            get => _selectedWoman;
            set => Set(ref _selectedWoman, value);
        }

        public AddSpouseViewModel(
            IFamilyTreeService familyService,
            IUserDialogService userDialog)
        {
            _familyService = familyService;
            _userDialog = userDialog;
            _ = LoadPeopleAsync(); // Загрузка людей при создании окна
        }

        // Загрузка людей из репозитория
        private async Task LoadPeopleAsync()
        {
            try
            {
                var people = (await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .ToList();

                // Группировка людей по полу
                _peopleByGender = people
                    .GroupBy(person => person.Gender)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Заполняем список мужчин
                Men.Clear();
                if (_peopleByGender.TryGetValue(Gender.Male, out var men))
                {
                    foreach (var man in men.OrderBy(p => p.Name))
                    {
                        Men.Add(man);
                    }
                }

                // Устанавливаем первый выбранный элемент
                SelectedMan = Men.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке людей: {ex.Message}");
            }
        }

        // Обновление списка женщин на основе выбранного мужчины
        private void UpdateWomenList()
        {
            Women.Clear();
            if (SelectedMan == null) return;

            if (_peopleByGender.TryGetValue(Gender.Female, out var women))
            {
                var filteredWomen = women
                    .Where(woman => IsAgeDifferenceValid(SelectedMan.DateOfBirth, woman.DateOfBirth)
                                    && !IsParentChildRelation(SelectedMan, woman))
                    .OrderBy(woman => woman.Name);

                foreach (var woman in filteredWomen)
                {
                    Women.Add(woman);
                }
            }

            // Устанавливаем первую женщину в списке
            SelectedWoman = Women.FirstOrDefault();
        }

        // Команда для создания супружеской связи
        private ICommand? _createRelationCommand;
        public ICommand CreateRelationCommand =>
            _createRelationCommand ??= new LambdaCommandAsync(CreateSpouseRelationAsync,
                () => SelectedMan is not null && SelectedWoman is not null);

        private async Task CreateSpouseRelationAsync()
        {
            if (SelectedMan == null || SelectedWoman == null) return;

            try
            {
                // Создание связи
                bool success = await _familyService.AddSpouseRelationAsync(SelectedMan, SelectedWoman);

                _userDialog.ShowInformation(success
                    ? $"Супруги {SelectedMan.Name} и {SelectedWoman.Name} успешно связаны."
                    : $"Не удалось установить связь между {SelectedMan.Name} и {SelectedWoman.Name}.");
                _userDialog.Close();
                SelectedMan = null;
                SelectedWoman = null;
            }
            catch (Exception ex)
            {
                _userDialog.ShowError($"Ошибка при добавлении супруга: {ex.Message}");
            }
        }

        // Проверка разницы в возрасте
        private bool IsAgeDifferenceValid(DateTime manBirthDate, DateTime womanBirthDate)
        {
            var ageDifference = Math.Abs(manBirthDate.Year - womanBirthDate.Year);
            return ageDifference <= 20;
        }

        // Проверка на родительско-детские отношения
        private bool IsParentChildRelation(PersonWrapper man, PersonWrapper woman)
        {
            return man.Children.Any(child => child.Id == woman.Id) ||
                   woman.Children.Any(child => child.Id == man.Id);
        }
    }
}
