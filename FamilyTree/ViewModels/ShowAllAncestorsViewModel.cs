using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.Models;
using MathCore.ViewModels;

namespace FamilyTree.Presentation.ViewModels
{
    public class ShowAllAncestorsViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;

        public string Title => "Показать всех предков";

        // Коллекция для отображения предков в TreeView
        public ObservableCollection<TNode> AncestorsTree { get; private set; } = new ObservableCollection<TNode>();

        // Выбранный человек
        private PersonWrapper? _selectedPerson;
        public PersonWrapper? SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (Set(ref _selectedPerson, value))
                    _ = LoadAncestorsAsync(); // Запускаем асинхронный метод без ожидания
            }
        }

        // Коллекция всех людей для выбора
        public ObservableCollection<PersonWrapper> People { get; private set; } = new ObservableCollection<PersonWrapper>();

        public ShowAllAncestorsViewModel(IFamilyTreeService familyService)
        {
            _familyService = familyService;
            _ = LoadPeopleAsync(); // Запускаем асинхронную загрузку людей
        }

        // Загрузка всех людей для выбора
        private async Task LoadPeopleAsync()
        {
            try
            {
                var people = await _familyService.LoadPeopleAsync();
                People.Clear();
                foreach (var person in people)
                {
                    People.Add(new PersonWrapper(person));
                }
                SelectedPerson = People.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки людей: {ex.Message}");
            }
        }

        // Загрузка предков и построение дерева
        public async Task LoadAncestorsAsync()
        {
            if (SelectedPerson == null)
            {
                Console.WriteLine("Выберите человека.");
                return;
            }

            try
            {
                // Получаем список всех предков
                var ancestors = await _familyService.GetAllAncestorsAsync(SelectedPerson);

                AncestorsTree.Clear();

                // Добавляем самого человека в корень дерева
                var rootNode = new TNode { Name = SelectedPerson.ToString() };
                AncestorsTree.Add(rootNode);

                // Строим дерево начиная с его предков
                foreach (var ancestor in ancestors)
                {
                    AddAncestorToTree(ancestor, ancestors, rootNode.Children);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки предков: {ex.Message}");
            }
        }

        // Добавление предка в дерево
        private void AddAncestorToTree(Person current, IEnumerable<Person> allAncestors, ObservableCollection<TNode> parentChildren)
        {
            // Создаем узел для текущего человека
            var node = new TNode { Name = current.ToString() };

            // Добавляем узел в родительскую коллекцию
            parentChildren.Add(node);

            // Находим детей текущего человека через FamilyRelation
            var children = allAncestors
                .Where(person => person.Parents != null && person.Parents.Any(p => p.Parent == current));

            // Рекурсивно добавляем детей как подузлы
            foreach (var child in children)
            {
                AddAncestorToTree(child, allAncestors, node.Children);
            }
        }

    }


    // Модель узла дерева
    public class TNode
    {
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<TNode> Children { get; set; } = new ObservableCollection<TNode>();
    }
}
