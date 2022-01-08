using System;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawBoardLines(400, 400);
            DrawCross(1, 1);
            DrawCircle(80, 80);
            DrawCircle(160, 80);
        }
        public void DrawCircle(double x, double y)
        {
            Ellipse circle = new Ellipse()
            {
                Width = 80,
                Height = 80,
                Stroke = System.Windows.Media.Brushes.Brown,
                StrokeThickness = 6,
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

            Line myLine = new System.Windows.Shapes.Line();
            myLine.Stroke = System.Windows.Media.Brushes.Brown;
            myLine.X1 = x;
            myLine.X2 = x + Consts.PieceSize;
            myLine.Y1 = y;
            myLine.Y2 = y + Consts.PieceSize;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 3;
            GameArea.Children.Add(myLine);

            Line myLine2 = new System.Windows.Shapes.Line();
            myLine2.Stroke = System.Windows.Media.Brushes.Brown;
            myLine2.X1 = x + Consts.PieceSize;
            myLine2.X2 = x;
            myLine2.Y1 = y;
            myLine2.Y2 = y + Consts.PieceSize;
            myLine2.HorizontalAlignment = HorizontalAlignment.Left;
            myLine2.VerticalAlignment = VerticalAlignment.Center;
            myLine2.StrokeThickness = 3;
            GameArea.Children.Add(myLine2);

            return true;
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
            Point p = e.GetPosition(GameArea);
            double x = p.X;
            double y = p.Y;
            (x,y) = FromCanvasCordsToBoardCords(x, y);
            (x,y) = FromBoardCordsToCanvasCords(x,y);

            DrawCircle(x,y);
        }
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
