using System.Windows;

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

#region radio button unchecks
        private void CirclePieceUnchecked(object sender, RoutedEventArgs e)
        {
            CirclePlayerType = TypeOfPlayer.Ai;
        }
        private void CrossPieceUnchecked(object sender, RoutedEventArgs e)
        {
            CrossPlayerType = TypeOfPlayer.Ai;
        }
                    
        private void HumanVsHumanUnchecked(object sender, RoutedEventArgs e)
        {
            TypeOfGame = TypeOfGame.HumanVsAi;
        }
        private void HumanVsAiUnchecked(object sender, RoutedEventArgs e)
        {
            TypeOfGame = TypeOfGame.HumanVsHuman;
        }
        private void EasyUnchecked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Easy;
        }
        private void MediumUnchecked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Medium;
        }
        private void HardUnchecked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Hard;
        }
        private void NormalUnchecked(object sender, RoutedEventArgs e)
        {
            DifficultyLevel = DifficultyLevel.Normal;
        }

#endregion

        private void WindowSubmit(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        #endregion

    }
}
