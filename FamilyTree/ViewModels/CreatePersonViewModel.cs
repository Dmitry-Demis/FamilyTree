using System.Collections.ObjectModel;
using System.Windows.Input;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.Models;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using MathCore.ViewModels;
using MathCore.WPF.Commands;

namespace FamilyTree.Presentation.ViewModels
{
    public class GenderItem
    {
        public Gender Gender { get; set; }
        public string DisplayName { get; set; }
    }

    public class CreatePersonViewModel
        (IFamilyTreeService familyService
        , IUserDialogService userDialog) : ViewModel
    {
        public ObservableCollection<string> Genders { get; } = new ObservableCollection<string>(Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(gender => gender.GetDisplayName()));
        public string Title { get; init; } = "Создать магазин";

        private readonly IFamilyTreeService _familyService = familyService ?? throw new ArgumentNullException(nameof(familyService));
        private readonly IUserDialogService _userDialog = userDialog ?? throw new ArgumentNullException(nameof(userDialog));

        private PersonWrapper _selectedPerson = new(new Person());
        public PersonWrapper SelectedPerson
        {
            get => _selectedPerson;
            set => Set(ref _selectedPerson, value);
        }

        private ICommand? _createPersonCommand;
        public ICommand CreatePersonCommand => _createPersonCommand ??= new LambdaCommandAsync(OnCreatePersonExecutedAsync, CanCreatePersonExecute);

        private bool CanCreatePersonExecute() =>
            Services.Extensions.StringExtensions.IsNotNullOrWhiteSpace(SelectedPerson.FullName);

        // Метод, вызываемый при выполнении команды
        private async Task OnCreatePersonExecutedAsync()
        {
            try
            {
                //// Если создание прошло успешно
                if (await _familyService.CreatePersonAsync(SelectedPerson))
                {
                    _userDialog.ShowInformation($"Человек \"{SelectedPerson.FullName}\" успешно создан.");
                }
            }
            catch (ArgumentException ex)
            {
                // Ошибка, если имя или адрес пустые
                _userDialog.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                // Общая ошибка при создании магазина
                _userDialog.ShowError($"Ошибка при создании магазина: {ex.Message}");
            }
            finally
            {
                // Закрытие диалога (если нужно)
                _userDialog.Close();
            }
        }
    }
}
