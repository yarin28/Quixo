using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quixo
{
    /// <summary>
    /// Interaction logic for BoardUi.xaml
    /// </summary>
    public partial class BoardUi : UserControl, INotifyPropertyChanged
    {

        enum BoardState { WaitingForSourcePieceSelection, WaitingForDestinationPiece };
        BoardState boardState;

        private Board board = new Board();
        List<System.Drawing.Point> validSources;
        List<System.Drawing.Point> validDestination;
        System.Drawing.Point srcP;
        private int boardPiecePixelDimension = 80;
       SmartEngine robot = new SmartEngine();

        public TypeOfPlayer CrossPlayerType { get; set; }
        public TypeOfPlayer CirclePlayerType { get; set; }

        public string CurrentPlayer
        {
            get
            {
                return board.CurrentPlayer.ToString();
            }
        }
        public string WinningPlayer
        {
            get
            {
                return board.WinningPlayer.ToString();
            }
        }

        public event Action<Player> PlayerWon;
        public event Action<Move> MoveMade
        {
            add { board.MoveMade += value; }
            remove { board.MoveMade -= value; }
        }
        public event Action<int> RobotMoveMadeReporter;
        public event PropertyChangedEventHandler? PropertyChanged;


        public void ChangeDifficulty(DifficultyLevel difficulty)
        {
            robot.SetDepthLimit((int)difficulty+2);//the +2 is for the minimax,
                                                   //maybe should not be here
                                                   //(its not that function job to deal with it. 
        }
        public BoardUi()
        {
            InitializeComponent();
            board.PropertyChanged += Board_PropertyChanged;
            board.Updated += UpdateUI;
        }
        #region event handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DrawBoard();
        }
        private void Board_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(board.WinningPlayer):
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinningPlayer)));
                    break;
                case nameof(board.CurrentPlayer):
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPlayer)));
                    break;
            }

        }
        # endregion
        private void Click(object sender, MouseButtonEventArgs e)
        {

            System.Windows.Point p = e.GetPosition(GameArea);
            p = acquireBoardPointsFromSystemWindowsPoint(p);
            //i have to types of Points, so a conversion is needed
            System.Drawing.Point dp = new System.Drawing.Point((int)p.X, (int)p.Y);
            if (boardState == BoardState.WaitingForSourcePieceSelection)
            {
                srcP = dp;//for the next phase of the game
                if (validSources.Contains(dp) == true)
                {
                    HightlightpossibleDestPieces(dp);
                    boardState = BoardState.WaitingForDestinationPiece;
                }
            }
            else if (boardState == BoardState.WaitingForDestinationPiece)
            {
                if (validDestination.Contains(dp) == true)
                {
                    Move playerCurrentMove = new Move(board.CurrentPlayer, srcP, dp);
                    //↑ i have to create a move object to pass it to the UpdateUi method
                    board.MovePiece(playerCurrentMove.Source, playerCurrentMove.Destination);
                    boardState = BoardState.WaitingForSourcePieceSelection;
                }
                else// if the destination is not valid
                {
                    boardState = BoardState.WaitingForSourcePieceSelection;
                    HightlightpossibleSourcePieces();
                }
            }
            if (IsCircleAi() || IsCrossAi())//its AI turn
            {
                _ = RobotMove();
            }
            if (board.WinningPlayer != Player.None)
            {
                PlayerWon?.Invoke(board.WinningPlayer);
                Reset();
            }
        }
        #region UI
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
            //HACK: the method is called from the board, so i have to convert the board points to canvas points
            //NOTE: there is a way to draw both lines with the same line object, but i dont know how to do it
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
            //FIXME: the color of the erase brush is not the same as the board color
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

            //HACK: there is a possability to draw the lines in the same loop.
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
        private void HightlightSelectedPiece(System.Drawing.Point src)
        {

            (src.X, src.Y) = FromBoardCordsToCanvasCords(src.X, src.Y);
            Rectangle rec = CreateSrcHighlightRectangle();
            rec.SetValue(Canvas.LeftProperty, (double)src.X + 2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)src.Y + 2);
        }
        private Rectangle CreateSrcHighlightRectangle()
        {
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.HorizontalAlignment = HorizontalAlignment.Left;
            rec.Stroke = System.Windows.Media.Brushes.Red;
            rec.StrokeThickness = 10;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = boardPiecePixelDimension - 5;
            rec.Width = boardPiecePixelDimension - 5;
            GameArea.Children.Add(rec);
            return rec;
        }

        private static (int, int) FromBoardCordsToCanvasCords(double x, double y)
        {
            int CanvasX = ((int)x * Consts.PieceSize);
            int CanvasY = 320 - ((int)y * Consts.PieceSize);
            return (CanvasX, CanvasY);
        }
        private static (int, int) FromCanvasCordsToBoardCords(double x, double y)
        {
            int BoardX = (int)x / Consts.PieceSize;
            int BoardY = (int)y / Consts.PieceSize;
            return (BoardX, BoardY);
        }
        #endregion

        #region Utils
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
        public void StartPlay()
        {

            if (CrossPlayerType == TypeOfPlayer.Ai)
            {
                AiPlay();
            }
            HightlightpossibleSourcePieces();
        }
        private void AiPlay()
        {
            Move robotMove = RobotMove();
        }
        private void Reset()
        {
            this.board.Reset();
            this.DrawBoard();
            HightlightpossibleSourcePieces();
        }
        private void UpdateUI()
        {
            this.DrawBoard();
            HightlightpossibleSourcePieces();
        }
        private bool IsCircleAi()
        {
            return board.CurrentPlayer == Player.O && this.CirclePlayerType == TypeOfPlayer.Ai;
        }
        private bool IsCrossAi()
        {
            return board.CurrentPlayer == Player.X && this.CrossPlayerType == TypeOfPlayer.Ai;
        }
        private Move RobotMove()
        {
            var stopWatch = Stopwatch.StartNew();
            Move robotMove = robot.GenerateMove(board);
            this.board.MovePiece(robotMove.Source, robotMove.Destination);
            stopWatch.Stop();
            RobotMoveMadeReporter?.Invoke((int)stopWatch.ElapsedMilliseconds);
            // NOTE: maybe not the best name↑
            return robotMove;
        }
        #endregion
    }

}

