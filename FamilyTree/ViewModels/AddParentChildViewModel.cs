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
    public class AddParentChildViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;
        private readonly IUserDialogService _userDialog;

        public string Title => "Добавить родителя";

        // Список родителей
        public ObservableCollection<PersonWrapper> Parents { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Список детей
        public ObservableCollection<PersonWrapper> Children { get; private set; } = new ObservableCollection<PersonWrapper>();

        // Выбранный родитель
        private PersonWrapper? _selectedParent;
        public PersonWrapper? SelectedParent
        {
            get => _selectedParent;
            set
            {
                if (null != value && Set(ref _selectedParent, value))
                {
                    // Обновить список детей, исключив выбранного родителя
                    UpdateChildrenList();
                }
            }
        }

        // Выбранный ребенок
        private PersonWrapper? _selectedChild;
        public PersonWrapper? SelectedChild
        {
            get => _selectedChild;
            set
            {
                if (Set(ref _selectedChild, value))
                {
                    // Логика при изменении выбранного ребенка, если требуется
                }
            }
        }

        public AddParentChildViewModel(
            IFamilyTreeService familyService
            , IUserDialogService userDialog)
        {
            _familyService = familyService;
            _userDialog = userDialog;
            _ = LoadParentsAsync(); // Загружаем только родителей
        }

        // Метод для загрузки родителей
        private async Task LoadParentsAsync()
        {
            try
            {
                var people = (await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .ToList();

                Parents.Clear();
                Children.Clear();

                // Добавляем всех людей как родителей
                foreach (var person in people)
                {
                    Parents.Add(person);
                    // Добавляем всех людей в список детей
                    Children.Add(person);
                }

                // Сортируем родителей и детей по алфавиту
                Parents = new ObservableCollection<PersonWrapper>(Parents.OrderBy(p => p.Name));
                Children = new ObservableCollection<PersonWrapper>(Children.OrderBy(c => c.Name));

                // Устанавливаем первого родителя по умолчанию
                SelectedParent = Parents.FirstOrDefault();

                // Устанавливаем первого доступного ребенка по умолчанию
                SelectedChild = Children.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке людей: {ex.Message}");
            }
        }

        // Метод для обновления списка детей, исключив выбранного родителя
        private void UpdateChildrenList()
        {
            if (SelectedParent != null)
            {
                // Очищаем текущий список детей
                Children.Clear();

                // Добавляем всех людей, кроме выбранного родителя
                foreach (var person in Parents)
                {
                    if (person != SelectedParent)
                    {
                        Children.Add(person);
                    }
                }

                // Если выбранный ребенок не присутствует в списке, сбрасываем его
                if (SelectedChild != null && !Children.Contains(SelectedChild))
                {
                    SelectedChild = null;
                }

                // Если после фильтрации детей выбранного ребенка нет, выбираем первого из оставшихся
                if (SelectedChild == null && Children.Any())
                {
                    SelectedChild = Children.FirstOrDefault();
                }
            }
        }


        private ICommand? _ParentChildConnectionCommand;
        public ICommand ParentChildConnectionCommand =>
        _ParentChildConnectionCommand ??= new LambdaCommandAsync(ConnectParentChildAsync);

        // Метод для установления связи родителя и ребенка
        private async Task ConnectParentChildAsync()
        {
            if (SelectedParent == null || SelectedChild == null)
            {
                // Сообщение об ошибке или предупреждение
                Console.WriteLine("Выберите родителя и ребенка.");
                return;
            }

            try
            {
                // Получаем объекты для родителя и ребенка
                var parent = SelectedParent;
                var child = SelectedChild;

                // Добавляем родителя к ребёнку и ребёнка к родителю в одном методе
                bool success = await _familyService.AddParentChildRelationAsync(parent, child);

                _userDialog.ShowInformation(success
                    ? $"Родитель {parent.Name} успешно добавлен к ребенку {child.Name}."
                    : $"Не удалось добавить родителя {parent.Name} к ребенку {child.Name}.");
                _userDialog.Close();
                SelectedParent = null;
                SelectedChild = null;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                Console.WriteLine($"Ошибка при добавлении родителя: {ex.Message}");
            }
        }

    }
}
