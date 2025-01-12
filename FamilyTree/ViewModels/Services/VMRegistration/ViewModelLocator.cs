using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.Presentation.ViewModels.Services.VMRegistration
{
    public class ViewModelLocator
    {
        private static T Resolve<T>() where T : class
            => App.Services.GetRequiredService<T>();

        public static MainWindowViewModel MainWindowViewModel => Resolve<MainWindowViewModel>();
        public static CreatePersonViewModel CreatePersonViewModel => Resolve<CreatePersonViewModel>();
        public static RemovePersonViewModel RemovePersonViewModel => Resolve<RemovePersonViewModel>();
        public static AddParentChildViewModel AddParentChildViewModel => Resolve<AddParentChildViewModel>();
        public static AddSpouseViewModel AddSpouseViewModel => Resolve<AddSpouseViewModel>();
        public static ShowClosestRelativesViewModel ShowClosestRelativesViewModel => Resolve<ShowClosestRelativesViewModel>();
        public static ShowAllAncestorsViewModel ShowAllAncestorsViewModel => Resolve<ShowAllAncestorsViewModel>();
        public static CalculateAncestorAgeViewModel CalculateAncestorAgeViewModel => Resolve<CalculateAncestorAgeViewModel>();
    }
}
