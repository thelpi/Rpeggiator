using System.Windows;

namespace RPG4.Visuals
{
    /// <summary>
    /// Logique d'interaction pour IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IntroWindow()
        {
            InitializeComponent();
            btnExit.Content = Messages.BtnExitMessage;
            btnNewGame.Content = Messages.BtnStartGameMessage;
            btnScreenEditor.Content = Messages.BtnScreenEditorMessage;
            txtRandomSentence.Text = "TODO : plagiat de minecraft à faire ici.";
        }

        private void btnScreenEditor_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            new ScreenEditorWindow().ShowDialog();
            ShowDialog();
        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            RpeggiatorLib.SqliteMapper.Defaut(Properties.Settings.Default.ResourcesPath).ResetDatabase(true);
            new MainWindow().ShowDialog();
            ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
