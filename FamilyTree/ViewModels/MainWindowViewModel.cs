using System.Collections.ObjectModel;
using System.Windows.Input;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.Views.Windows;
using MathCore.ViewModels;
using MathCore.WPF.Commands;

namespace FamilyTree.Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyTreeService;
        public string Title => "Генеалогическое древо";

        private ICommand? _createPersonCommand;

        public ICommand CreatePersonCommand =>
            _createPersonCommand ??= new LambdaCommand(App.OpenWindow<CreatePersonWindow>);


        private ICommand? _removePersonCommand;

        public ICommand RemovePersonCommand =>
            _removePersonCommand ??= new LambdaCommand(App.OpenWindow<RemovePersonWindow>);

        public ObservableCollection<Person> FamilyTree { get; } = new ObservableCollection<Person>();
        public MainWindowViewModel(IFamilyTreeService familyTreeService)
        {
            _familyTreeService = familyTreeService ?? throw new ArgumentNullException(nameof(familyTreeService));
            _ = LoadFamilyTreeAsync();
        }

        // Метод для загрузки семейного дерева
        public async Task LoadFamilyTreeAsync()
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
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                // Можно добавить уведомление об ошибке пользователю
                Console.WriteLine(ex.Message);
            }
        }

        private ICommand? _AddParentCommand;
        public ICommand AddParentCommand =>
            _AddParentCommand ??= new LambdaCommand(App.OpenWindow<AddParentChildWindow>);

        private ICommand? _AddSpouseCommand;
        public ICommand AddSpouseCommand =>
            _AddSpouseCommand ??= new LambdaCommand(App.OpenWindow<AddSpouseWindow>);
    }
}
