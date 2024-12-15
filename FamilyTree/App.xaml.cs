using System.Windows;
using FamilyTree.BLL.Services;
using FamilyTree.DAL.StorageRegistration;
using FamilyTree.Presentation.ViewModels.Services.Dialogs;
using FamilyTree.Presentation.ViewModels.Services.VMRegistration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FamilyTree.Presentation
{
    public partial class App
    {
        private static IHost? _host;

        private static IHost Host => _host
            ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
        public static IServiceProvider Services => Host.Services;
        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddStorage(host.Configuration)
            .AddViewModels()
            .AddWindowsUserDialogs()
            .AddServices()

        ;
        protected override async void OnStartup(StartupEventArgs e)
        {
            var host = Host;
            base.OnStartup(e);
            await host.StartAsync();
        }
        protected override async void OnExit(ExitEventArgs e)
        {
            using var host = Host;
            base.OnExit(e);
            await host.StopAsync();
        }

        public static void OpenWindow<TWindow>() where TWindow : Window, new()
        {
            var window = new TWindow { Owner = Current.MainWindow };
            window.ShowDialog();
        }
    }
}