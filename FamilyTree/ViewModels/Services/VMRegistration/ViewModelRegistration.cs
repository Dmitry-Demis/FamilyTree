using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.Presentation.ViewModels.Services.VMRegistration
{
    internal static class ViewModelRegistration
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
           .AddScoped<MainWindowViewModel>()
           .AddTransient<CreatePersonViewModel>()
           .AddTransient<RemovePersonViewModel>()
           .AddTransient<AddParentChildViewModel>()
           .AddTransient<AddParentChildViewModel>()
           .AddTransient<AddSpouseViewModel>()
        ;
    }
}
