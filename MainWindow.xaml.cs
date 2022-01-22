using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
//TODO: all the imports are not used should be gone
namespace Quixo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private Board board = new Board();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            board.SetPiece(3, 3, Player.O);
            board.SetPiece(4, 4, Player.O);
            board.SetPiece(2, 2, Player.O);
            board.SetPiece(1, 1, Player.O);
            List<System.Drawing.Point> points =  board.EfficiantBoardDrawCrossPoints();
            points.AddRange(board.EfficiantBoardDrawCirclePoints());
            foreach (System.Drawing.Point p in points)
                {
                int x, y;
                //(x,y) = FromBoardCordsToCanvasCords((double)p.X,(double)p.Y);
                DrawCross(p.X, p.Y);
            }
            DrawBoardLines(400, 400);
            //DrawCross(0, 0);
            //DrawCircle(80, 80);
            //DrawCircle(160, 80);
            //ErasePiece(1,1);
            //Highlight(0,0);
            //ErasePiece(0, 0);
        }
        public void DrawBoard()
        {

            GameArea.Children.Clear(); 
            List<Piece> points =  board.EfficiantBoradDrawAllPoints();
            foreach (Piece p in points)
                {
                int x, y;
                if (p.Player == Player.X) DrawCross(p.Position.X, p.Position.Y);
                else DrawCircle(p.Position.X, p.Position.Y);
            }
            DrawBoardLines(400, 400);
        }
        public void DrawCircle(int x, int y)
        {
            (x,y)=FromBoardCordsToCanvasCords(x,y);
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
        public Boolean DrawCross(int x, int y)
        {
            //TODO there is a problem that the gameArea canvas object cant be
            //passed as a reference so the drawing must be done inside the
            //[./MainWindow.xaml.cs]
            (x,y)=FromBoardCordsToCanvasCords(x,y);
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
            (x,y)=FromBoardCordsToCanvasCords(x, y);
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.Fill= new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0xe1, 0xc1, 0x6e)); ;
            rec.HorizontalAlignment = HorizontalAlignment.Left;
                rec.Stroke = System.Windows.Media.Brushes.Brown ;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = 80;//must be changed
            rec.Width = 80;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)x);
            rec.SetValue(Canvas.TopProperty, (double)y);
        }
        private void Highlight(int x,int y)
        {
            (x, y) = FromBoardCordsToCanvasCords(x, y);
            System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle();
            rec.HorizontalAlignment = HorizontalAlignment.Left;
                rec.Stroke = System.Windows.Media.Brushes.Yellow ;
            rec.StrokeThickness = 10;
            rec.Cursor = Cursors.Cross;
            rec.VerticalAlignment = VerticalAlignment.Center;
            rec.Height = 80-5;//must be changed
            rec.Width = 80-5;
            GameArea.Children.Add(rec);
            rec.SetValue(Canvas.LeftProperty, (double)x+2);//must be changed to const
            rec.SetValue(Canvas.TopProperty, (double)y+2);
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
            int x =(int) p.X;
            int y =(int) p.Y;
            (x,y) = FromCanvasCordsToBoardCords(x, y);

            DrawCircle(x,y);
            DrawCross(x, y);
        }
        //===================utilty members======================
        private static (int,int) FromBoardCordsToCanvasCords(double x, double y)
        {
            int CanvasX = (int)x * Consts.PieceSize;
            int CanvasY = (int)y * Consts.PieceSize;
            return (CanvasX, CanvasY);
        }
        private static (int,int)FromCanvasCordsToBoardCords(double x, double y)
        {
            int BoardX = (int)x / Consts.PieceSize;
            int BoardY = (int)y / Consts.PieceSize;
            return (BoardX, BoardY);
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
