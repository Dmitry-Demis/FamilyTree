using FamilyTree.Presentation.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MathCore.ViewModels;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using System.Windows.Input;
using FamilyTree.Presentation.Views.Windows;
using MathCore.WPF.Commands;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;

namespace FamilyTree.Presentation.ViewModels
{
    public class CalculateAncestorAgeViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        public string Title => "Вычислить возраст предка при рождении потомка";

        // Список предков
        public ObservableCollection<PersonWrapper> Ancestors { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Список потомков
        public ObservableCollection<PersonWrapper> Descendants { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Выбранный предок
        private PersonWrapper? _selectedAncestor;
        public PersonWrapper? SelectedAncestor
        {
            get => _selectedAncestor;
            set
            {
                if (null != value && Set(ref _selectedAncestor, value))
                {

                }
            }
        }

        // Выбранный потомок
        private PersonWrapper? _selectedDescendant;
        public PersonWrapper? SelectedDescendant
        {
            get => _selectedDescendant;
            set
            {
                Set(ref _selectedDescendant, value);
            }
        }

        public CalculateAncestorAgeViewModel(
            IFamilyTreeService familyService,
            IUserDialogService userDialog)
        {
            _familyService = familyService;
            _userDialog = userDialog;
            _ = LoadPeopleAsync();
        }

        // Загрузка предков и потомков
        private async Task LoadPeopleAsync()
        {
            try
            {
                var people = (await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .ToList();

                Ancestors.Clear();
                Descendants.Clear();

                foreach (var person in people)
                {
                    Ancestors.Add(person);
                    Descendants.Add(person);
                }

                // Сортировка по дате рождения, полу и имени
                Ancestors = new ObservableCollection<PersonWrapper>(
                    Ancestors.OrderBy(p => p.DateOfBirth)
                             .ThenBy(p => p.Gender)
                             .ThenBy(p => p.Name));

                Descendants = new ObservableCollection<PersonWrapper>(
                    Descendants.OrderBy(p => p.DateOfBirth)
                               .ThenBy(p => p.Gender)
                               .ThenBy(p => p.Name));

                SelectedAncestor = Ancestors.FirstOrDefault();
                SelectedDescendant = Descendants.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        
        private ICommand? _CalculateAgeCommand;
        public ICommand CalculateAgeCommand =>
            _CalculateAgeCommand ??= new LambdaCommandAsync(CalculateAgeAsync, () => SelectedAncestor != null && SelectedDescendant != null);

        // Метод для вычисления возраста предка при рождении потомка
        private async Task CalculateAgeAsync()
        {
            if (SelectedAncestor == null || SelectedDescendant == null)
            {
                Console.WriteLine("Необходимо выбрать предка и потомка.");
                return;
            }

            try
            {
                var ancestor = SelectedAncestor;
                var descendant = SelectedDescendant;

                // Получаем всех предков выбранного потомка
                var ancestors = await _familyService.GetAllAncestorsAsync(descendant.Model);

                // Проверяем, является ли выбранный предок в списке предков
                if (ancestors.All(a => a.Id != ancestor.Id))
                {
                    _userDialog.ShowError($"Выбранный предок {ancestor.Name} не является предком для потомка {descendant.Name}.");
                    return;
                }

                // Вычисляем возраст предка при рождении потомка
                var ageAtBirth = descendant.DateOfBirth.Year - ancestor.DateOfBirth.Year;
                if (descendant.DateOfBirth < ancestor.DateOfBirth.AddYears(ageAtBirth))
                {
                    ageAtBirth--;
                }

                _userDialog.ShowInformation($"Возраст предка {ancestor.Name} при рождении потомка {descendant.Name}: {ageAtBirth} лет.");
                _userDialog.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка вычисления возраста: {ex.Message}");
            }
        }

    }
}
