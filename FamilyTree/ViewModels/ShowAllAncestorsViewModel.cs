using FamilyTree.BLL.Services;
using FamilyTree.Presentation.Models;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using MathCore.WPF.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MathCore.WPF.ViewModels;
using FamilyTree.DAL.Model;

namespace FamilyTree.Presentation.ViewModels
{
    public class ShowAllAncestorsViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;

        // Список людей
        public ObservableCollection<PersonWrapper> People { get; set; } = new ObservableCollection<PersonWrapper>();

        // Выбранный человек
        private PersonWrapper? _selectedPerson;
        public PersonWrapper? SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (Set(ref _selectedPerson, value))
                {
                    _ = LoadAncestorsAsync(); // Загружаем предков при изменении выбора
                }
            }
        }

        // Список предков
        public ObservableCollection<TreeNode> TreeNodes { get; private set; } = new ObservableCollection<TreeNode>();

        // Команда для отображения предков
        private ICommand? _showAllAncestorsCommand;
        public ICommand ShowAllAncestorsCommand =>
            _showAllAncestorsCommand ??= new LambdaCommandAsync(ShowAllAncestorsAsync, () => SelectedPerson != null);

        // Конструктор
        public ShowAllAncestorsViewModel(IFamilyTreeService familyService)
        {
            _familyService = familyService;
            _ = LoadPeopleAsync(); // Загрузка всех людей при инициализации
        }

        // Загрузка людей
        private async Task LoadPeopleAsync()
        {
            try
            {
                var people = await _familyService.LoadPeopleAsync();
                People = new ObservableCollection<PersonWrapper>(people.Select(person => new PersonWrapper(person)).OrderBy(p => p.Name));

                // Для первого выбранного человека показываем предков
                SelectedPerson = People.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке списка людей: {ex.Message}");
            }
        }

        // Загрузка предков
        private async Task LoadAncestorsAsync()
        {
            if (SelectedPerson == null) return;

            try
            {
                var ancestors = new List<TreeNode>();
                var level = 0;

                // Добавляем самого человека на первый уровень
                ancestors.Add(new TreeNode
                {
                    Name = SelectedPerson.ToString(),
                    Level = level,
                    X = level * 120, // Расположение по горизонтали с учетом уровня
                    Y = ancestors.Count * 100 // Расположение по вертикали
                });

                // Получаем всех предков для выбранного человека
                var allAncestors = await _familyService.GetAllAncestorsAsync(SelectedPerson);

                // Сортировка предков по уровням
                foreach (var ancestor in allAncestors)
                {
                    ancestors.Add(new TreeNode
                    {
                        Name = ancestor.ToString(),
                        Level = ++level,
                        X = level * 120, // Расположение по горизонтали с учетом уровня
                        Y = ancestors.Count * 100 // Расположение по вертикали
                    });
                }

                // Обновляем TreeNodes для отображения
                TreeNodes.Clear();
                foreach (var ancestor in ancestors)
                {
                    TreeNodes.Add(ancestor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке предков: {ex.Message}");
            }
        }


        // Асинхронное выполнение команды отображения предков
        private async Task ShowAllAncestorsAsync()
        {
            if (SelectedPerson == null)
            {
                Console.WriteLine("Выберите человека, чтобы увидеть его предков.");
                return;
            }

            try
            {
                await LoadAncestorsAsync();
                Console.WriteLine($"Все предки для {SelectedPerson.Name} успешно загружены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отображении предков: {ex.Message}");
            }
        }
    }



}
