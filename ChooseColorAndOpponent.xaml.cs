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

        public TypeOfPlayer crossPlayerType = TypeOfPlayer.Ai;
        public TypeOfPlayer circlePlayerType = TypeOfPlayer.Ai;
        public TypeOfGame typeOfGame = TypeOfGame.HumanVsHuman;
        public DifficultyLevel difficultyLevel = DifficultyLevel.Normal;
        #region getters and setters
        public TypeOfPlayer CrossPlayerType
        {
            get { return crossPlayerType; }
            set { crossPlayerType = value; }
        }
        public TypeOfPlayer CirclePlayerType
        {
            get { return circlePlayerType; }
            set { circlePlayerType = value; }
        }
        public TypeOfGame TypeOfGame
        {
            get { return typeOfGame; }
            set { typeOfGame = value; }
        }
        public DifficultyLevel DifficultyLevel
        {
            get { return difficultyLevel; }
            set { difficultyLevel = value; }
        }
        #endregion

        public ChooseColorAndOpponent()
        {
            InitializeComponent();
        }

        #region event handlers
        private void HumanVsAiButtonClicked(object sender, RoutedEventArgs e)
        {
            TypeOfGame = TypeOfGame.HumanVsAi;
        }

        private void HumanVsHumanButtonClicked(object sender, RoutedEventArgs e)
        {
            TypeOfGame= TypeOfGame.HumanVsHuman;
        }

        private void CrossPieceClicked(object sender, RoutedEventArgs e)
        {
            CrossPlayerType = TypeOfPlayer.Human;
        }

        private void CirclePieceClicked(object sender, RoutedEventArgs e)
        {
            CirclePlayerType = TypeOfPlayer.Human;
        }
        private void EasyClicked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Easy;
        }
        private void NormalClicked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Normal;
        }
        private void MediumClicked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Medium;
        }
        private void HardClicked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Hard;
        }

        private void WindowSubmit(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
        #endregion

    }
}
