using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
