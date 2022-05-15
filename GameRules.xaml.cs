using System.Windows;
using System.Windows.Controls;

namespace Quixo
{
    /// <summary>
    /// Interaction logic for GameRules.xaml
    /// </summary>
    public partial class GameRules : Window
    {
        public GameRules()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        public string MyPic
        {
            get { return @"https://img.fruugo.com/product/3/95/143031953_max.jpg"; }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
