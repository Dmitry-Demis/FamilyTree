using FamilyTree.Presentation.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MathCore.ViewModels;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.Model;
using System.Windows.Input;
using MathCore.WPF.Commands;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;

namespace FamilyTree.Presentation.ViewModels
{
    public class TreeNode
    {
        public string Name { get; set; } = string.Empty; // Имя узла (человека)
        public double X { get; set; } // Координата X
        public double Y { get; set; } // Координата Y
    }

    public class Link
    {
        public double StartX { get; set; } // Начальная координата X
        public double StartY { get; set; } // Начальная координата Y
        public double EndX { get; set; } // Конечная координата X
        public double EndY { get; set; } // Конечная координата Y
    }

    public class ShowClosestRelativesViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        public string Title => "Просмотр ближайших родственников";

        public ObservableCollection<PersonWrapper> People { get; set; } = new ObservableCollection<PersonWrapper>(); // Инициализация коллекции People
        // Список родителей
        public ObservableCollection<PersonWrapper> Parents { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Список детей
        public ObservableCollection<PersonWrapper> Children { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Выбранный человек
        private PersonWrapper? _selectedPerson;
        public PersonWrapper? SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (Set(ref _selectedPerson, value))
                {
                    _ = LoadRelativesAsync(); // Обновляем список родственников при изменении выбора
                }
            }
        }

        public ShowClosestRelativesViewModel(
            IFamilyTreeService familyService,
            IUserDialogService userDialog)
        {
            _familyService = familyService;
            _userDialog = userDialog;
            _ = LoadPeopleAsync(); // Загрузка всех людей при инициализации
        }

        // Метод для загрузки списка людей
        private async Task LoadPeopleAsync()
        {
            try
            {
                People = new ObservableCollection<PersonWrapper>((await _familyService.LoadPeopleAsync())
                        .Select(person => new PersonWrapper(person))
                        .OrderBy(p => p.Name));

                SelectedPerson = People.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке списка людей: {ex.Message}");
            }
        }

        // Метод для загрузки родственников
        private async Task LoadRelativesAsync()
        {
            if (SelectedPerson == null) return;

            try
            {
                // Получение детей
                var children = (await _familyService.GetChildrenAsync(SelectedPerson)).ToList();
                // Получение родителей (предполагаю, что нужен метод GetParentsAsync)
                var parents = (await _familyService.GetParentsAsync(SelectedPerson)).ToList(); // Исправленный метод

                if (children.Any())
                {
                    Children.Clear(); // Очищаем текущий список
                    foreach (var child in children.Where(p => p != null)) // Исключаем null-значения
                    {
                        Children.Add(new PersonWrapper(child!)); // Создаём обёртки
                    }
                }
                else
                {
                    Children.Clear(); // Очищаем список, если детей нет
                }

                if (parents.Any())
                {
                    Parents.Clear(); // Очищаем текущий список
                    foreach (var parent in parents.Where(p => p != null)) // Исключаем null-значения
                    {
                        Parents.Add(new PersonWrapper(parent!)); // Создаём обёртки
                    }
                }
                else
                {
                    Parents.Clear(); // Очищаем список, если родителей нет
                }

                // Генерируем дерево
                GenerateTree();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке родственников: {ex.Message}");
            }
        }

        private ICommand? _showRelativesCommand;
        public ICommand ShowRelativesCommand =>
            _showRelativesCommand ??= new LambdaCommandAsync(ShowRelativesAsync, () => SelectedPerson != null);

        // Команда для отображения ближайших родственников
        private async Task ShowRelativesAsync()
        {
            if (SelectedPerson == null)
            {
                _userDialog.ShowWarning("Выберите человека, чтобы увидеть его родственников.");
                return;
            }

            try
            {
                await LoadRelativesAsync();
                _userDialog.ShowInformation($"Ближайшие родственники для {SelectedPerson.Name} успешно загружены.");
            }
            catch (Exception ex)
            {
                _userDialog.ShowError($"Ошибка при отображении родственников: {ex.Message}");
            }
        }

        // Свойства для канваса
        private double _canvasWidth;
        private double _canvasHeight;

        public double CanvasWidth
        {
            get => _canvasWidth;
            set => Set(ref _canvasWidth, value);
        }

        public double CanvasHeight
        {
            get => _canvasHeight;
            set => Set(ref _canvasHeight, value);
        }

        public ObservableCollection<TreeNode> TreeNodes { get; private set; } = new ObservableCollection<TreeNode>();
        public ObservableCollection<Link> Links { get; private set; } = new ObservableCollection<Link>();

        // Генерация дерева с расчётом координат
        private void GenerateTree()
        {
            TreeNodes.Clear();
            Links.Clear(); // Очистка списка связей (стрелочек)

            if (SelectedPerson == null) return;

            const double boxWidth = 120;
            const double boxHeight = 50;
            const double horizontalSpacing = 40;
            const double verticalSpacing = 80;

            // Задаём размеры канваса (например, исходя из ширины экрана)
            double canvasWidth = 800;  // Ширина канваса
            double canvasHeight = 600; // Высота канваса

            // Центр канваса
            double canvasCenterX = canvasWidth / 2;
            double centerY = 100; // Центр Y (можно варьировать)

            // Добавляем центральный узел
            var centralNode = new TreeNode
            {
                Name = $"{SelectedPerson.Name} (выбранный)",
                X = canvasCenterX - boxWidth / 2,
                Y = centerY
            };
            TreeNodes.Add(centralNode);

            // Добавляем родителей
            double parentY = centerY - verticalSpacing;
            double parentX = canvasCenterX - (Parents.Count - 1) * (boxWidth + horizontalSpacing) / 2;

            foreach (var parent in Parents)
            {
                var parentNode = new TreeNode
                {
                    Name = $"{parent.Name} (родитель)",
                    X = parentX,
                    Y = parentY
                };
                TreeNodes.Add(parentNode);

                // Добавляем связь (стрелочку) от родителя к центральному узлу
                //Links.Add(new Link
                //{
                //    StartX = parentNode.X + boxWidth / 2,
                //    StartY = parentNode.Y + boxHeight,
                //    EndX = centralNode.X + boxWidth / 2,
                //    EndY = centralNode.Y
                //});

                parentX += boxWidth + horizontalSpacing;
            }

            // Добавляем детей
            double childY = centerY + verticalSpacing;
            double childX = canvasCenterX - (Children.Count - 1) * (boxWidth + horizontalSpacing) / 2;

            foreach (var child in Children)
            {
                var childNode = new TreeNode
                {
                    Name = $"{child.Name} (ребёнок)",
                    X = childX,
                    Y = childY
                };
                TreeNodes.Add(childNode);

                // Добавляем связь (стрелочку) от центрального узла к ребёнку
                //Links.Add(new Link
                //{
                //    StartX = centralNode.X + boxWidth / 2,
                //    StartY = centralNode.Y + boxHeight,
                //    EndX = childNode.X + boxWidth / 2,
                //    EndY = childNode.Y
                //});

                childX += boxWidth + horizontalSpacing;
            }

            // Обновляем размеры канваса
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
        }

    }
}
