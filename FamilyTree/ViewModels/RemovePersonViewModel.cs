using FamilyTree.Presentation.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks; // Для async/await
using MathCore.ViewModels;
using FamilyTree.BLL.Services;
using MathCore.WPF.Commands;
using System.Windows.Input;
using FamilyTree.DAL.Model;

namespace FamilyTree.Presentation.ViewModels
{
    public class RemovePersonViewModel : ViewModel
    {
        private readonly IFamilyTreeService _familyService;

        public string Title => "Удалить человека";

        // Список людей, с которыми работает приложение
        public ObservableCollection<PersonWrapper> People { get; } = [];

        // Выбранный человек
        private PersonWrapper? _selectedPerson = new(new Person());
        public PersonWrapper? SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                if (Set(ref _selectedPerson, value))
                {
                    // Здесь можно добавить логику, которая будет выполняться при изменении выбранного человека
                }
            }
        }

        // Текст поиска
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && Set(ref _searchText, value))
                {
                    UpdateSelectedPerson(value);
                }
            }
        }

        // Обновление выбранного человека по тексту поиска
        private void UpdateSelectedPerson(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                SelectedPerson = null;
                return;
            }

            var updatedPerson = People
                .FirstOrDefault(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            SelectedPerson = updatedPerson;
        }

        // Команда для удаления человека
        public ICommand RemovePersonCommand { get; }

        public RemovePersonViewModel(IFamilyTreeService familyService)
        {
            _familyService = familyService;

            // Инициализация команды для удаления
            RemovePersonCommand = new LambdaCommand(RemovePerson, CanRemovePerson);

            // Загружаем людей при инициализации ViewModel
           _ = LoadPeopleAsync(); // Загружаем людей асинхронно
        }

        // Логика удаления человека
        private void RemovePerson()
        {
            if (SelectedPerson != null)
            {
                People.Remove(SelectedPerson);
                SelectedPerson = null; // Очистить выбор
            }
        }

        // Условия для разрешения команды удаления
        private bool CanRemovePerson()
        {
            return SelectedPerson != null;
        }

        // Асинхронный метод для загрузки людей
        private async Task LoadPeopleAsync()
        {
            try
            {
                // Загружаем людей асинхронно через сервис
                var people = (await _familyService.LoadPeopleAsync())
                    .Select(person => new PersonWrapper(person))
                    .ToList()
                    ;

                // Очищаем коллекцию перед добавлением новых данных
                People.Clear();
                People.AddRange(people);
                SelectedPerson = null;
            }
            catch (Exception ex)
            {
                // Обработка ошибок при загрузке данных
                // Можно показать ошибку пользователю через интерфейс или журналировать
                // Например: 
                // MessageBox.Show($"Ошибка при загрузке людей: {ex.Message}");
            }
        }
    }
}
