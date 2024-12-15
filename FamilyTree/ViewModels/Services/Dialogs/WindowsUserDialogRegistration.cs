using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.Presentation.ViewModels.Services.Dialogs
{
    internal static class WindowsUserDialogRegistration
    {
        public static IServiceCollection AddWindowsUserDialogs(this IServiceCollection services) => services
            .AddSingleton<IUserDialogService, WindowsUserDialogService>()
            ;
    }
}
