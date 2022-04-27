using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
namespace Quixo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        enum BoardState { WaitingForSourcePieceSelection, WaitingForDestanetionPiece };
        enum TypesOfPlayer { Ai, Human };
        BoardState boardState;
        private Board board = new Board();
        List<System.Drawing.Point> validSources;
        List<System.Drawing.Point> validDestination;
        System.Drawing.Point srcP;
        private TypesOfPlayer CrossPlayerType;
        private TypesOfPlayer CirclePlayerType;
        private int boardPiecePixelDimension = 80;
        SmartEngine robot = new SmartEngine();
        public string GetCurrentPlayer
        {
            get
            {
                return board.CurrentPlayer.ToString();
            }
        }
        public string GetWinningPlayer
        {
            get
            {
                return board.WinningPlayer.ToString();
            }
        }
        public MainWindow()
        {
            SelectPieceAndTypeOfGameWithPopUpWindow();
            InitializeComponent();
            this.DataContext = this;
        }
        private void Click(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Point p = e.GetPosition(GameArea);
            p = acquireBoardPointsFromSystemWindowsPoint(p);
            System.Drawing.Point dp = new System.Drawing.Point((int)p.X, (int)p.Y);
            if (boardState == BoardState.WaitingForSourcePieceSelection)
            {
                srcP = dp;
                if (validSources.Contains(dp) == true)
                {
                    HightlightpossibleDestPieces(dp);
                    boardState = BoardState.WaitingForDestanetionPiece;
                }
            }
            else if (boardState == BoardState.WaitingForDestanetionPiece)
            {
                if (validDestination.Contains(dp) == true)
                {
                    Move playerCurrentMove = new Move(board.CurrentPlayer, srcP, dp);
                    board.MovePiece(playerCurrentMove.Source, playerCurrentMove.Destination);
                    boardState = BoardState.WaitingForSourcePieceSelection;
                    UpdateUI(playerCurrentMove);
                }
                else
                {
                    boardState = BoardState.WaitingForSourcePieceSelection;
                    HightlightpossibleSourcePieces();
                }
            }//NOTE should switch between the if`s placement
             //NOTE: big bad hack
            if (IsCircleAi() || IsCrossAi())
            {
                Move robotMove = RobotMove();
                UpdateUI(robotMove);
            }
            ifOnePlayerWon();
        }

        private void ifOnePlayerWon()
        {
            if (this.board.WinningPlayer != Player.None)
            {
                GameWon g = new GameWon(this.board.WinningPlayer.ToString());
                g.ShowDialog();
                this.board.Reset();
                ResetUi();
                SelectPieceAndTypeOfGameWithPopUpWindow();
                DrawBoard();
            StartAiFirstPlayer();
            HightlightpossibleSourcePieces();
                

            }
        }

        private void StartAiFirstPlayer()
        {
            if (CrossPlayerType == TypesOfPlayer.Ai)
            {
                Move robotMove = RobotMove();
                UpdateUI(robotMove);
            }
        }
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
                    CrossPlayerType = TypesOfPlayer.Human;
                    CirclePlayerType = TypesOfPlayer.Ai;
                }
                else
                {
                    CrossPlayerType = TypesOfPlayer.Ai;
                    CirclePlayerType = TypesOfPlayer.Human;
                }
            }
            else
            {
                CrossPlayerType = TypesOfPlayer.Human;
                CirclePlayerType = TypesOfPlayer.Human;
            }
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {

            DrawBoard();
            StartAiFirstPlayer();
            HightlightpossibleSourcePieces();

        }
        #region UI members
        public void HightlightpossibleSourcePieces()
        {
            validSources = this.board.GetValidSourcePieces();
            foreach (System.Drawing.Point p in validSources) Highlight(p);
        }
        public void HightlightpossibleDestPieces(System.Drawing.Point source)
        {

            DrawBoard();
            HightlightSelectedPiece(source);
            validDestination = this.board.GetValidDestinationPieces(source);
            foreach (System.Drawing.Point p in validDestination) Highlight(p);
        }
        public void DrawBoard()
        {

            GameArea.Children.Clear();
            List<Piece> points = board.EfficiantBoradDrawAllPoints();
            foreach (Piece p in points)
            {
                if (p.Player == Player.X) DrawCross(p.Position.X, p.Position.Y);
                else DrawCircle(p.Position.X, p.Position.Y);
            }
            DrawBoardLines(400, 400);
        }
        public void DrawCircle(int x, int y)
        {
            (x, y) = FromBoardCordsToCanvasCords(x, y);
            Ellipse circle = new Ellipse()
            {
                Width = boardPiecePixelDimension,
                Height = boardPiecePixelDimension,
                Stroke = System.Windows.Media.Brushes.Brown,
                StrokeThickness = 5,
            };

            GameArea.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (double)x);
            circle.SetValue(Canvas.TopProperty, (double)y);

        }
        public bool DrawCross(int x, int y)
        {
            (x, y) = FromBoardCordsToCanvasCords(x, y);
            Line myLine = new System.Windows.Shapes.Line();
            myLine.Stroke = System.Windows.Media.Brushes.Brown;
            myLine.X1 = x;
            myLine.X2 = x + Consts.PieceSize;
            myLine.Y1 = y;
            myLine.Y2 = y + Consts.PieceSize;
            myLine.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 5;
            GameArea.Children.Add(myLine);

            Line myLine2 = new System.Windows.Shapes.Line();
            myLine2.Stroke = System.Windows.Media.Brushes.Brown;
            myLine2.X1 = x + Consts.PieceSize;
            myLine2.X2 = x;
            myLine2.Y1 = y;
            myLine2.Y2 = y + Consts.PieceSize;
            myLine2.HorizontalAlignment = HorizontalAlignment.Left;
            myLine2.VerticalAlignment = VerticalAlignment.Center;
            myLine2.StrokeThickness = 5;
            GameArea.Children.Add(myLine2);

            return true;
        }
        private void ErasePiece(int x, int y)
        {
            (x, y) = FromBoardCordsToCanvasCords(x, y);
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xe1, 0xc1, 0x6e)); ;
            rec.HorizontalAlignment = HorizontalAlignment.Left;
            rec.Stroke = System.Windows.Media.Brushes.Brown;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = boardPiecePixelDimension;
            rec.Width = boardPiecePixelDimension;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)x);
            rec.SetValue(Canvas.TopProperty, (double)y);
        }
        private void Highlight(System.Drawing.Point p)
        {
            (p.X, p.Y) = FromBoardCordsToCanvasCords(p.X, p.Y);
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.HorizontalAlignment = HorizontalAlignment.Left;
            rec.Stroke = System.Windows.Media.Brushes.Yellow;
            rec.StrokeThickness = 10;
            //rec.Cursor = Cursors.Cross;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = boardPiecePixelDimension - 5;
            rec.Width = boardPiecePixelDimension - 5;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)p.X + 2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)p.Y + 2);
        }
        private void DrawBoardLines(int width, int hight)
        {

            for (int i = 0; i <= hight; i += hight / 5)
            {
                Line myLine = new System.Windows.Shapes.Line();
                myLine.Stroke = System.Windows.Media.Brushes.Brown;
                myLine.X1 = i;
                myLine.X2 = i;
                myLine.Y1 = 0;
                myLine.Y2 = hight;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 3;
                GameArea.Children.Add(myLine);
            }


            for (int j = 0; j <= width; j += width / 5)
            {

                Line myLine = new System.Windows.Shapes.Line();
                myLine.Stroke = System.Windows.Media.Brushes.Brown;
                myLine.X1 = 0;
                myLine.X2 = width;
                myLine.Y1 = j;
                myLine.Y2 = j;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 2;
                GameArea.Children.Add(myLine);
            }
        }
        private void UpdateUI(Move robotMove)
        {
            this.DrawBoard();
            HightlightpossibleSourcePieces();
            MoveTable.Items.Add(new PrintableMove(robotMove));
            currentPlayerLable.Content = board.CurrentPlayer.ToString();
            winningPlayerLable.Content = board.WinningPlayer.ToString();
        }
        private void ResetUi()
        {
            this.DrawBoard();
            HightlightpossibleSourcePieces();
            MoveTable.Items.Clear();
            currentPlayerLable.Content = board.CurrentPlayer.ToString();
            winningPlayerLable.Content = board.WinningPlayer.ToString();
        }
        #endregion 
        #region utility members
        private bool IsCircleAi()
        {
            return board.CurrentPlayer == Player.O && this.CirclePlayerType == TypesOfPlayer.Ai;
        }
        private bool IsCrossAi()
        {
            return board.CurrentPlayer == Player.X && this.CrossPlayerType == TypesOfPlayer.Ai;
        }
        private Move RobotMove()
        {
            Move robotMove = robot.GenerateMove(board);
            this.board.MovePiece(robotMove.Source, robotMove.Destination);
            return robotMove;
        }
        private static Point acquireBoardPointsFromSystemWindowsPoint(Point p)
        {
            //converting the points from the original canvas
            //x,y to more general points that can be used in
            //all the functions.
            p.X = (int)p.X;
            p.Y = (int)p.Y;
            (p.X, p.Y) = FromCanvasCordsToBoardCords(p.X, p.Y);
            p.Y = 4 - p.Y;
            return p;
        }
        private void HightlightSelectedPiece(System.Drawing.Point src)
        {

            (src.X, src.Y) = FromBoardCordsToCanvasCords(src.X, src.Y);
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.HorizontalAlignment = HorizontalAlignment.Left;
            rec.Stroke = System.Windows.Media.Brushes.Red;
            rec.StrokeThickness = 10;
            //rec.Cursor = Cursors.Cross;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = boardPiecePixelDimension - 5;
            rec.Width = boardPiecePixelDimension - 5;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)src.X + 2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)src.Y + 2);
        }
        private static (int, int) FromBoardCordsToCanvasCords(double x, double y)
        {
            int CanvasX = ((int)x * Consts.PieceSize);// i want to use a const but this func will be called too many time
            int CanvasY = 320 - ((int)y * Consts.PieceSize);// and that will "cost" to much
            return (CanvasX, CanvasY);
        }
        private static (int, int) FromCanvasCordsToBoardCords(double x, double y)
        {
            int BoardX = (int)x / Consts.PieceSize;
            int BoardY = (int)y / Consts.PieceSize;
            return (BoardX, BoardY);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GameRules win2 = new GameRules();
            win2.Show();
        }
        private void AboutButton(object sender,RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }
        private void GameRulesButton(object sender, RoutedEventArgs e)
        {
            GameRules win2 = new GameRules();
            win2.Show();
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
        private void MoveTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            About w = new About();
            w.Show();
        }
        #endregion
    }
}

