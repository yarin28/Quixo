using System;
using System.Windows;
namespace Quixo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window    {
        enum BoardState { WaitingForSourcePieceSelection, WaitingForDestinationPiece };
        BoardState boardState;


        public string DebugTextBox { get; set; }
        #region Window starters
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
                            
        /// <summary>
        /// event that is trigged when the window is rendered
        /// </summary>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SelectPieceAndTypeOfGameWithPopUpWindow();
            boardUi.StartPlay();

        }
        /// <summary>
        /// opens a window that prompts the player to select-
        ///     1. piece type
        ///     2. game type(AI vs Human/Human vs Human)
        ///     3. AI difficulty
        /// </summary>
        private void SelectPieceAndTypeOfGameWithPopUpWindow()
        {
            //Note: the Window object does not have all the methods that i used,
            // so i used the object itself.
            ChooseColorAndOpponent startWindow = new ChooseColorAndOpponent();
            //set the players type according the startWindow options
            startWindow.ShowDialog();
            if (startWindow.typeOfGame == TypeOfGame.HumanVsHuman)
            {
                boardUi.CrossPlayerType = TypeOfPlayer.Human;
                boardUi.CirclePlayerType = TypeOfPlayer.Human;
            }
            else
            {
                boardUi.CrossPlayerType = startWindow.CrossPlayerType;
                boardUi.CirclePlayerType = startWindow.CirclePlayerType;
            }
            boardUi.ChangeDifficulty(startWindow.DifficultyLevel);
        }
        #endregion
        #region event handlers
        /*
            all the event are subscribed to the boardUi,
            so i can use the boardUi methods to update the UI.
            they are subscribed using the XAML code.
        */
        private void MoveMade(Move robotMove)
        {
            MoveTable.Items.Add(new PrintableMove(robotMove));
        }
       private void PlayerWon(Player winner)
        {
            GameWon window = new GameWon(winner.ToString());
            window.ShowDialog();
            ResetUi();
        }
        private void RobotMoveMade(int ms)
        {
            debugTextBox.AppendText($"robot took - {ms} ms\n");
        }
        #endregion
        #region UI members
        /// <summary>
        /// reset all the <b>non </b> board members
        /// </summary>
        private void ResetUi()
        {
            MoveTable.Items.Clear();
            debugTextBox.Document.Blocks.Clear();
            SelectPieceAndTypeOfGameWithPopUpWindow();
        }
        #endregion 
        #region utility members
        /// <summary>
        /// event to open the <b>about</b> button
        /// </summary>
        private void AboutButton(object sender, RoutedEventArgs e)
        {
            About w = new About();
            w.Show();
        }
        /// <summary>
        /// event to open the <b>game rules</b> button
        /// </summary>
        private void GameRulesButton(object sender, RoutedEventArgs e)
        {
            GameRules win2 = new GameRules();
            win2.Show();
        }
        #endregion
    }
}
