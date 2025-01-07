using System.Collections.ObjectModel;
using System.Windows.Input;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using FamilyTree.Presentation.Views.Windows;
using MathCore.ViewModels;
using MathCore.WPF.Commands;
using QuickGraph;

namespace FamilyTree.Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyTreeService;
        private readonly IUserDialogService _userDialog;
        public string Title => "Генеалогическое древо";

        private ICommand? _createPersonCommand;

        public ICommand CreatePersonCommand =>
            _createPersonCommand ??= new LambdaCommand(App.OpenWindow<CreatePersonWindow>);


        private ICommand? _removePersonCommand;

        public ICommand RemovePersonCommand =>
            _removePersonCommand ??= new LambdaCommand(App.OpenWindow<RemovePersonWindow>);

        public ObservableCollection<Person> FamilyTree { get; } = new ObservableCollection<Person>();

       
        public MainWindowViewModel(
            IFamilyTreeService familyTreeService
            , IUserDialogService userDialog)
        {
            _familyTreeService = familyTreeService ?? throw new ArgumentNullException(nameof(familyTreeService));
            _userDialog = userDialog;
            _ = LoadFamilyTreeAsync();

            // Инициализируем HTML-контент
            // Инициализация графа
            _familyGraph = new BidirectionalGraph<string, Edge<string>>();

            // Пример добавления элементов в граф (генеалогия)
            _familyGraph.AddVertex("Иван Иванов");
            _familyGraph.AddVertex("Мария Сидорова");
            _familyGraph.AddEdge(new Edge<string>("Иван Иванов", "Мария Сидорова"));

            _familyGraph.AddVertex("Петр Петров");
            _familyGraph.AddEdge(new Edge<string>("Иван Иванов", "Петр Петров"));
            // Обновляем граф через уведомление
            OnPropertyChanged(nameof(FamilyGraph));

        }

        // Граф (связи) для отображения
        private BidirectionalGraph<string, Edge<string>> _familyGraph;
        public BidirectionalGraph<string, Edge<string>> FamilyGraph
        {
            get => _familyGraph;
            set => Set(ref _familyGraph, value);
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

       
        private ICommand? _AddParentCommand;
        public ICommand AddParentCommand =>
            _AddParentCommand ??= new LambdaCommand(App.OpenWindow<AddParentChildWindow>);

        private ICommand? _AddSpouseCommand;
        public ICommand AddSpouseCommand =>
            _AddSpouseCommand ??= new LambdaCommand(App.OpenWindow<AddSpouseWindow>);


    }
}
