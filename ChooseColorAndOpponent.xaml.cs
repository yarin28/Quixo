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
    /// Interaction logic for ChooseColorAndOpponent_.xaml
    /// </summary>
    public partial class ChooseColorAndOpponent : Window
    {
        private bool humanVsAi = false;
        private bool humanVsHuman = false;
        private bool cross = false;
        private bool circle = false;

        public bool HumanVsAi { get => humanVsAi; set => humanVsAi = value; }
        public bool HumanVsHuman { get => humanVsHuman; set => humanVsHuman = value; }
        public bool Cross { get => cross; set => cross = value; }
        public bool Circle { get => circle; set => circle = value; }

        public ChooseColorAndOpponent()
        {
            InitializeComponent();
        }

        private void HumanVsAiButtonClicked(object sender, RoutedEventArgs e)
        {
            HumanVsAi = !this.humanVsAi;
        }

        private void HumanVsHumanButtonClicked(object sender, RoutedEventArgs e)
        {
            HumanVsHuman = !this.humanVsHuman;
        }

        private void CrossPieceClicked(object sender, RoutedEventArgs e)
        {
            Cross = !this.cross;
        }

        private void CirclePieceClicked(object sender, RoutedEventArgs e)
        {
            Circle = !this.circle;
        }

        private void WindowSubmit(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}
