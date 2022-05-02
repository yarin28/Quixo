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
        #region Window starters
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }
                            
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SelectPieceAndTypeOfGameWithPopUpWindow();
            boardUi.StartPlay();

        }
        private void SelectPieceAndTypeOfGameWithPopUpWindow()
        {
            //Note: the Window object does not have all the methods that i used,
            // so i used the object itself.
            ChooseColorAndOpponent startWindow = new ChooseColorAndOpponent();
            //set the players type according the startWindow options
            startWindow.ShowDialog();
        boardUi.CrossPlayerType = startWindow.CrossPlayerType;
        boardUi.CirclePlayerType = startWindow.CirclePlayerType;
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
            debugTextBox.AppendText($"robot took - {ms}\n");
        }
        #endregion
        #region UI members
        private void ResetUi()
        {
            MoveTable.Items.Clear();
            debugTextBox.Document.Blocks.Clear();
            SelectPieceAndTypeOfGameWithPopUpWindow();
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
        #endregion
    }
}
