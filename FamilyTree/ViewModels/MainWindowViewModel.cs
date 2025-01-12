using System.Collections.ObjectModel;
using System.Windows.Input;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using FamilyTree.Presentation.Views.Windows;
using MathCore.ViewModels;
using MathCore.WPF.Commands;


namespace FamilyTree.Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyTreeService;
        private readonly IUserDialogService _userDialog;
        public static string Title => "Генеалогическое древо";

        private ICommand? _createPersonCommand;

        public ICommand CreatePersonCommand =>
            _createPersonCommand ??= new LambdaCommand(App.OpenWindow<CreatePersonWindow>);


        private ICommand? _removePersonCommand;

        public ICommand RemovePersonCommand =>
            _removePersonCommand ??= new LambdaCommand(App.OpenWindow<RemovePersonWindow>);


        private ICommand? _showClosestRelativesCommand;

        public ICommand ShowClosestRelativesCommand =>
            _showClosestRelativesCommand ??= new LambdaCommand(App.OpenWindow<ShowClosestRelativesWindow>);

        private ObservableCollection<Person> FamilyTree { get; } = [];

       
        public MainWindowViewModel(
            IFamilyTreeService familyTreeService
            , IUserDialogService userDialog)
        {
            _familyTreeService = familyTreeService ?? throw new ArgumentNullException(nameof(familyTreeService));
            _userDialog = userDialog;
            _ = LoadFamilyTreeAsync();

            

        }

       
        // Метод для загрузки семейного дерева
        private async Task LoadFamilyTreeAsync()
        {
            try
            {
                // Загрузка данных
                var people = await _familyTreeService.LoadPeopleAsync();

                // Очистка и добавление загруженных данных в ObservableCollection
                FamilyTree.Clear();
                foreach (var person in people)
                {
                    FamilyTree.Add(person);
                }
                // После загрузки данных обновляем 
                // Выводим HTML в консоль для проверки
                // Программное обновление источника для WebView2

            }
            catch (Exception ex)
            {
                // Обработка ошибок
                // Можно добавить уведомление об ошибке пользователю
                Console.WriteLine(ex.Message);
            }
        }

       
        private ICommand? _addParentCommand;
        public ICommand AddParentCommand =>
            _addParentCommand ??= new LambdaCommand(App.OpenWindow<AddParentChildWindow>);

        private ICommand? _addSpouseCommand;
        public ICommand AddSpouseCommand =>
            _addSpouseCommand ??= new LambdaCommand(App.OpenWindow<AddSpouseWindow>);

        private ICommand? _showAllAncestorsCommand;
        public ICommand ShowAllAncestorsCommand =>
            _showAllAncestorsCommand ??= new LambdaCommand(App.OpenWindow<ShowAllAncestorsWindow>);

        private ICommand? _calculateAgeCommand;

        public ICommand CalculateAgeCommand
            =>
                _calculateAgeCommand ??= new LambdaCommand(App.OpenWindow<CalculateAncestorAgeWindow>);

        private ICommand? _clearDbCommand;

        public ICommand ClearDbCommand
            =>
                _clearDbCommand ??= new LambdaCommand(() => _familyTreeService.DeleteDatabaseAsync());
    }
}
