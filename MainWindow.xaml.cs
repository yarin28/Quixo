using System;
using System.ComponentModel;
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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
                            
                            
        /// <summary>
        /// main method of the game, it is called when the user clicks on the board
        /// the method checks the board state and calls the appropriate methods
        /// </summary>

        private void SelectPieceAndTypeOfGameWithPopUpWindow()
        {
            //Note: the Window object does not have all the methods that i used,
            // so i used the object itself.
            ChooseColorAndOpponent startWindow = new ChooseColorAndOpponent();
            //set the players type according the startWindow options
            startWindow.ShowDialog();
            bool isAi = startWindow.HumanVsAi;
            if (isAi)
            {
                if (startWindow.Cross)
                {
                    boardUi.CrossPlayerType = TypesOfPlayer.Human;
                    boardUi.CirclePlayerType = TypesOfPlayer.Ai;
                }
                else
                {
                    boardUi.CrossPlayerType = TypesOfPlayer.Ai;
                    boardUi.CirclePlayerType = TypesOfPlayer.Human;
                }
            }
            else
            {
                boardUi.CrossPlayerType = TypesOfPlayer.Human;
                boardUi.CirclePlayerType = TypesOfPlayer.Human;
            }
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SelectPieceAndTypeOfGameWithPopUpWindow();
            boardUi.StartPlay();

        }
        #region UI members
        private void MoveMade(Move robotMove)
        {
            //FIXME:use events to update all the labels
            MoveTable.Items.Add(new PrintableMove(robotMove));
        }
        /// <summary>
        ///NOTE:this method relies on prev function
        /// </summary>
        private void ResetUi()
        {
            //this.DrawBoard();
            //HightlightpossibleSourcePieces();
            //MoveTable.Items.Clear();
            //currentPlayerLable.Content = board.CurrentPlayer.ToString();
            //winningPlayerLable.Content = board.WinningPlayer.ToString();
        }
        #endregion 
        #region utility members
        private void AboutButton(object sender, RoutedEventArgs e)
        {
            About w = new About();
            w.Show();
        }
        private void GameRulesButton(object sender, RoutedEventArgs e)
        {
            GameRules win2 = new GameRules();
            win2.Show();
        }
       private void PlayerWon(Player winnter)
        {
            GameWon window = new GameWon(winnter.ToString());
            window.ShowDialog();
            //FIXME: add a reset and reset the movetable.
        }
        public class PrintableMove
        {
            public string player
            { get; set; }
            public string source
            { get; set; }
            public string destination
            { get; set; }
            public PrintableMove(Move mov)
            {
                this.player = mov.Player.ToString();
                this.source = mov.Source.ToString();
                this.destination = mov.Destination.ToString();
            }

            public PrintableMove(Player player, System.Drawing.Point source, System.Drawing.Point dest)
            {
                this.player = player.ToString();
                this.source = source.ToString();
                this.destination = dest.ToString();
            }
        }
        #endregion
    }
}

