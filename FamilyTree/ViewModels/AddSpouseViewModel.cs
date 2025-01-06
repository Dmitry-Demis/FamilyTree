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

namespace FamilyTree.Presentation.ViewModels
{
    public class AddSpouseViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        public string Title => "Добавить супругов";

        // Коллекция мужчин
        public ObservableCollection<PersonWrapper> Men { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Коллекция женщин
        public ObservableCollection<PersonWrapper> Women { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Выбранный мужчина
        private PersonWrapper? _selectedMan;
        public PersonWrapper? SelectedMan
        {
            get => _selectedMan;
            set => Set(ref _selectedMan, value);
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
            _ = LoadPeopleAsync(); // Загрузка мужчин и женщин
        }

        // Загрузка людей и разделение их на мужчин и женщин
        private async Task LoadPeopleAsync()
        {
            try
            {
                var people = (await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .ToList();

                Men.Clear();
                Women.Clear();

                foreach (var person in people)
                {
                    switch (person)
                    {
                        case { Gender: Gender.Male }:
                            Men.Add(person);
                            break;
                        case { Gender: Gender.Female }:
                            Women.Add(person);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // Сортировка коллекций
                Men = new ObservableCollection<PersonWrapper>(Men.OrderBy(p => p.Name));
                Women = new ObservableCollection<PersonWrapper>(Women.OrderBy(p => p.Name));

                // Установка первых значений
                SelectedMan = Men.FirstOrDefault();
                SelectedWoman = Women.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке людей: {ex.Message}");
            }
        }

        private ICommand? _createRelationCommand;
        public ICommand CreateRelationCommand =>
            _createRelationCommand ??= new LambdaCommandAsync(CreateSpouseRelationAsync, ()=> SelectedMan is not null && SelectedWoman is not null);

        // Установление связи между мужчиной и женщиной
        private async Task CreateSpouseRelationAsync()
        {
            if (SelectedMan == null || SelectedWoman == null)
            {
                Console.WriteLine("Выберите мужчину и женщину для установления связи.");
                return;
            }

            try
            {
                var man = SelectedMan;
                var woman = SelectedWoman;

                // Установить супружескую связь
                bool success = await _familyService.AddSpouseRelationAsync(man, woman);

                _userDialog.ShowInformation(success
                    ? $"Супруги {man.Name} и {woman.Name} успешно связаны."
                    : $"Не удалось установить связь между {man.Name} и {woman.Name}.");
                _userDialog.Close();
                SelectedMan = null;
                SelectedWoman = null;
            }
            catch (Exception ex)
            {
                _userDialog.ShowError($"Ошибка при добавлении супруга: {ex.Message}");
            }
        }
    }
}
