using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.Models;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using MathCore.ViewModels;
using MathCore.WPF.Commands;

namespace FamilyTree.Presentation.ViewModels
{
    public class CreatePersonViewModel : ViewModel
    {
        public static string Title => "Создать человека";

        // Инициализация списка полов через Enum
        public ObservableCollection<Gender> Genders { get; } = new(
            Enum.GetValues(typeof(Gender)).Cast<Gender>()
        );

        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        // Инициализация выбранного человека
        private PersonWrapper _selectedPerson = new(new Person());
        public PersonWrapper SelectedPerson
        {
            get => _selectedPerson;
            set => Set(ref _selectedPerson, value);
        }

        // Ленивая инициализация команды
        private ICommand? _createPersonCommand;
        public ICommand CreatePersonCommand => _createPersonCommand ??= new LambdaCommandAsync(OnCreatePersonExecutedAsync, CanCreatePersonExecute);

        // Конструктор
        public CreatePersonViewModel(IFamilyTreeService familyService, IUserDialogService userDialog)
        {
            _familyService = familyService ?? throw new ArgumentNullException(nameof(familyService));
            _userDialog = userDialog ?? throw new ArgumentNullException(nameof(userDialog));

            // Установка даты рождения по умолчанию, если она не задана
            SelectedPerson.DateOfBirth = SelectedPerson.DateOfBirth == DateTime.MinValue
                ? new DateTime(1800, 1, 1)
                : SelectedPerson.DateOfBirth;
        }

        // Условие для активации команды создания человека
        private bool CanCreatePersonExecute()
        {
            // Проверка, что имя задано и дата рождения корректна
            return !string.IsNullOrWhiteSpace(SelectedPerson.Name) &&
                   SelectedPerson.DateOfBirth >= new DateTime(1800, 1, 1) &&
                   SelectedPerson.DateOfBirth <= DateTime.Now;
        }

        // Метод для выполнения создания человека
        private async Task OnCreatePersonExecutedAsync()
        {
            try
            {
                // Проверка успешности создания человека
                if (await _familyService.AddPersonToTreeAsync(SelectedPerson))
                {
                    _userDialog.ShowInformation($"Человек \"{SelectedPerson.Name}\" успешно создан.");
                }
            }
            catch (ArgumentException ex)
            {
                // Отображение ошибки, если имя или адрес пустые
                _userDialog.ShowError($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Общая ошибка при создании
                _userDialog.ShowError($"Ошибка при создании человека: {ex.Message}");
            }
            finally
            {
                // Закрытие диалога, если нужно
                _userDialog.Close();
            }
        }
    }
}
