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
using System.ComponentModel;

namespace FamilyTree.Presentation.ViewModels
{

    public class TNode
    {
        public string Name { get; set; } = string.Empty; // Имя узла
        public ObservableCollection<TNode> Children { get; set; } = new ObservableCollection<TNode>(); // Дети узла
        public double X { get; set; } // Координата X для отображения
        public double Y { get; set; } // Координата Y для отображения
        public int Level { get; set; } // Уровень узла в дереве

        public TNode(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }


public class ShowAllAncestorsViewModel : INotifyPropertyChanged
    {
        private readonly IFamilyTreeService _familyService;

        public ObservableCollection<TNode> TreeNodes { get; set; } = new ObservableCollection<TNode>();
        public ObservableCollection<PersonWrapper> People { get; set; } = new ObservableCollection<PersonWrapper>(); // Коллекция людей
        private PersonWrapper? _selectedPerson; // Приватное поле для SelectedPerson

        public PersonWrapper? SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (_selectedPerson != value)
                {
                    _selectedPerson = value;
                    OnPropertyChanged(nameof(SelectedPerson)); // Вызов события изменения свойства
                    LoadAncestorsAsync(_selectedPerson); // Загружаем предков, если выбран человек
                }
            }
        }

        public ShowAllAncestorsViewModel(IFamilyTreeService familyService)
        {
            _familyService = familyService;
            _ = LoadPeopleAsync();
        }

        // Метод для загрузки людей
        public async Task LoadPeopleAsync()
        {
            try
            {
                // Загружаем список людей и оборачиваем их в PersonWrapper
                People = new ObservableCollection<PersonWrapper>((await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .OrderBy(p => p.Name));

                // Устанавливаем первого человека в качестве выбранного
                SelectedPerson = People.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке списка людей: {ex.Message}");
            }
        }

        // Метод для загрузки и построения всех предков
        public async Task LoadAncestorsAsync(Person? selectedPerson)
        {
            if (selectedPerson == null)
                return;

            // Получаем все предков текущего человека
            var allAncestors = await _familyService.GetAllAncestorsAsync(selectedPerson);

            // Создаем корневой узел
            var rootNode = new TNode(selectedPerson.ToString(), 0);

            // Группировка предков по уровням
            foreach (var ancestor in allAncestors)
            {
                var level = 1;
                var ancestorNode = new TNode(ancestor.ToString(), level);
                rootNode.Children.Add(ancestorNode);

                // Добавляем родителей и сдвигаем их по уровням
                var parents = await _familyService.GetParentsAsync(ancestor);

                foreach (var parent in parents)
                {
                    if (parent != null)
                    {
                        var parentNode = new TNode(parent.ToString(), level + 1);
                        ancestorNode.Children.Add(parentNode);

                        // Получаем супруга и добавляем его рядом
                        var spouse = await _familyService.GetSpouseAsync(parent.SpouseId);
                        if (spouse != null)
                        {
                            var spouseNode = new TNode(spouse.ToString(), level + 1);
                            ancestorNode.Children.Add(spouseNode);
                        }
                    }
                }
            }

            // Устанавливаем позиции для всех узлов
            PositionTreeNodes(rootNode, 0, 0);

            // Обновляем TreeNodes для отображения
            TreeNodes.Clear();
            TreeNodes.Add(rootNode);
        }

        // Рекурсивная позиция узлов
        private void PositionTreeNodes(TNode node, double x, double y)
        {
            node.X = x;
            node.Y = y;

            // Горизонтальное смещение для каждого ребенка
            double offsetX = x;
            double offsetY = y + 100; // Отступ по вертикали для следующего уровня

            foreach (var child in node.Children)
            {
                PositionTreeNodes(child, offsetX, offsetY);
                offsetX += 150; // Горизонтальное распределение между детьми
            }
        }

        // Событие изменения свойства для INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }





}
