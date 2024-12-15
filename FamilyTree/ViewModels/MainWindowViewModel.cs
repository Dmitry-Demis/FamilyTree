using System.Collections.ObjectModel;
using System.Windows.Input;
using FamilyTree.DAL.Model;
using FamilyTree.Presentation.Views.Windows;
using MathCore.ViewModels;
using MathCore.WPF.Commands;

namespace FamilyTree.Presentation.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        public string Title { get; init; } = "Генеалогическое древо";

        private ICommand? _createPersonCommand;

        public ICommand CreatePersonCommand =>
            _createPersonCommand ??= new LambdaCommand(App.OpenWindow<CreatePersonWindow>);

        public async Task<ObservableCollection<Person>> LoadFamilyTreeFromDatabaseAsync()
        {
            // Пример загрузки данных из базы данных
            //var people = await _dbContext.People
            //    .Include(p => p.Children) // Подключаем детей
            //    .ToListAsync();

            //var familyTree = people.Select(p => new Person
            //{
            //    Name = p.Name,
            //    Children = new ObservableCollection<Person>(p.Children.Select(c => new Person { Name = c.Name }))
            //}).ToList();

            //return new ObservableCollection<Person>(familyTree);
            return new ObservableCollection<Person>();
        }
    }
}
