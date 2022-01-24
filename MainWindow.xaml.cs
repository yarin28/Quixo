using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
//TODO: all the imports are not used should be gone
namespace Quixo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        enum BoardState { WaitingForSourcePieceSelection, WaitingForDestanetionPiece };
        BoardState boardState;//HACK should find a better name.
        private Board board = new Board();
        List<System.Drawing.Point> validSources;
        List<System.Drawing.Point> validDestanation;
        System.Drawing.Point srcP;
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
            InitializeComponent();
            this.DataContext = this;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {

            DrawBoard();
            HightlightpossibleSourcePieces();
            //DrawCross(0, 0);
            //DrawCircle(80, 80);
            //DrawCircle(160, 80);
            //ErasePiece(1,1);
            //Highlight(0,0);
            //ErasePiece(0, 0);
        }
        public void HightlightpossibleSourcePieces()
        {
            validSources = this.board.GetValidSourcePieces();
            foreach (System.Drawing.Point p in validSources) Highlight(p);
        }
        public void HightlightpossibleDestPieces(System.Drawing.Point source)
        {

            DrawBoard();
            HightlightSelectedPiece(source);
            validDestanation = this.board.GetValidDestinationPieces(source);
            foreach (System.Drawing.Point p in validDestanation) Highlight(p);
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
                Width = 80,
                Height = 80,
                Stroke = System.Windows.Media.Brushes.Brown,
                StrokeThickness = 5,
            };

            GameArea.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (double)x);
            circle.SetValue(Canvas.TopProperty, (double)y);

        }
        public bool DrawCross(int x, int y)
        {
            //TODO there is a problem that the gameArea canvas object cant be
            //passed as a reference so the drawing must be done inside the
            //[./MainWindow.xaml.cs]
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
            rec.Height = 80;//must be changed
            rec.Width = 80;//HACK must be changed
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
            rec.Height = 80 - 5;//must be changed
            rec.Width = 80 - 5;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)p.X + 2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)p.Y + 2);
        }
        private void DrawBoardLines(int width, int hight)
        {
            /*TODOS:
             * 1 -  the edge lines are not as thicj as the middle lines,
             *      the myLine.StrokeThickness is to blame, the drawing should
             *      be starting in 1 and ending in length-1
             * 2-
             *      there is no need for 2 loops, there has to be a way to simplify 
             *      the code to allow 1 for loop.
             * 
             */

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
        private void GameArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(GameArea);
            //converting the points from the original canvas
            //x,y to more general points that can be used in
            //all the functions.
            p.X = (int)p.X;
            p.Y = (int)p.Y;
            (p.X, p.Y) = FromCanvasCordsToBoardCords(p.X, p.Y);
            p.Y = 4 - p.Y;
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
             if (boardState == BoardState.WaitingForDestanetionPiece)
            {
                if (validDestanation.Contains(dp) == true)
                {
                    board.MovePiece(srcP,dp);
                        this.DrawBoard();
                    boardState = BoardState.WaitingForSourcePieceSelection;
                    HightlightpossibleSourcePieces();
                    //HACK for preformence sake this better but its not opp
                    MoveTable.Items.Add(new PrintableMove(board.CurrentPlayer,srcP,dp));
                    currentPlayerLable.Content = board.CurrentPlayer.ToString();
                    winningPlayerLable.Content = board.WinningPlayer.ToString();
                }
            }//NOTE should switch between the if`s placement
            
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
            rec.Height = 80 - 5;//must be changed
            rec.Width = 80 - 5;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)src.X + 2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)src.Y + 2);
        }
        //===================utility members======================
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
            About win2 = new About();
                win2.Show();
        }

        private void GameRules_button(object sender, RoutedEventArgs e)
        {
            About win2 = new About();
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

            public PrintableMove(Player player,System.Drawing.Point source, System.Drawing.Point dest)
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
    }
}

/*
 * NOTES-
 * clear all the board => GameArea.Children.Clear();
 * 
 * 
 * 
 */
