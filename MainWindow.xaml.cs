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
using System.Windows.Navigation;
using System.Windows.Shapes;
//TODO: all the imports are not used should be gone
namespace Quixo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int SnakeSquareSize = 20;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DrawBoard(400,400);
        }

        private void DrawBoard(int width,int hight)
        {
        /*TODOS:
         * 1 -  the edge lines are not as thicj as the middle lines,
         *      the myLine.StrokeThickness is to blame, the drawing should
         *      be starting in 1 and ending in length-1
         * 2-
         *      there is no need for 2 loops, there has to be a way to simplify 
         *      the code to allow 1 for loop.
         * 
         * 
         */

            for(int i = 0; i <= hight; i+=hight/5)
            {
                  Line  myLine = new System.Windows.Shapes.Line();
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


                for(int j=0;j<=width; j+=width/5)
                {

                  Line  myLine = new System.Windows.Shapes.Line();
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
        private void DrawGameArea()
        {
            bool doneDrawingBackground = false;
            int nextX = 0, nextY = 0;
            int rowCounter = 0;
            bool nextIsOdd = false;

            while (doneDrawingBackground == false)
            {
                Rectangle rect = new Rectangle
                {
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
                    Fill = nextIsOdd ? Brushes.White : Brushes.Black
                };
                GameArea.Children.Add(rect);
                Canvas.SetTop(rect, nextY);
                Canvas.SetLeft(rect, nextX);

                nextIsOdd = !nextIsOdd;
                nextX += SnakeSquareSize;
                if (nextX >= GameArea.ActualWidth)
                {
                    nextX = 0;
                    nextY += SnakeSquareSize;
                    rowCounter++;
                    nextIsOdd = (rowCounter % 2 != 0);
                }

                if (nextY >= GameArea.ActualHeight)
                    doneDrawingBackground = true;
            }
        }
    }
}
