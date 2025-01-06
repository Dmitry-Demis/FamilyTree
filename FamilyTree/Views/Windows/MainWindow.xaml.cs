using System.Configuration;
using System.IO;
using System.Windows;

namespace FamilyTree.Presentation.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //// Инициализация WebView2
            //await WebView.EnsureCoreWebView2Async();

            //var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

            //var fullDbPath = Path.Combine(basePath, @"Assets\family_tree.html");
            //Directory.CreateDirectory(Path.GetDirectoryName(fullDbPath)!);

            //var s = File.Exists(@"X:\Learning\Programming\LW4\FamilyTree\FamilyTree\Assets\family_tree.html");
            //// Путь к вашему локальному HTML файлу
            //Uri fileUri = new Uri(fullDbPath);

            //// Установка источника для WebView2
            //WebView.Source = fileUri;

            //// Сообщение для взаимодействия с WebView2 (опционально)
            //WebView.CoreWebView2.WebMessageReceived += (s, e) =>
            //{
            //    var message = e.TryGetWebMessageAsString();
            //    // Обработка сообщения
            //};
        }
    }
}