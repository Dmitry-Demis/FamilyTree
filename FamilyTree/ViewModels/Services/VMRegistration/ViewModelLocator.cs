using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.Presentation.ViewModels.Services.VMRegistration
{
    public class ViewModelLocator
    {
        private static T Resolve<T>() where T : class
            => App.Services.GetRequiredService<T>();

        public static MainWindowViewModel MainWindowViewModel => Resolve<MainWindowViewModel>();
        public static CreatePersonViewModel CreatePersonViewModel => Resolve<CreatePersonViewModel>();
    }
}
