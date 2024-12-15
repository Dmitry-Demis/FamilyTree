using System.Windows.Input;
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
    }
}
