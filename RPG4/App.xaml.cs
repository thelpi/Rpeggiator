using System.Windows;

namespace RPG4
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Visuals.MainWindow().ShowDialog();
        }
    }
}
