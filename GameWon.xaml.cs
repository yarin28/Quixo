using System.Windows;

namespace Quixo
{
    /// <summary>
    /// Interaction logic for GameWon.xaml
    /// </summary>
    public partial class GameWon : Window
    {
        string winner;
        public GameWon(string winner)
        {
            InitializeComponent();
            this.winner = winner;
            this.DataContext = this;
        }
        public string GetWinner
        {
            get
            {
                return this.winner;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
