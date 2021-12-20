using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
namespace Quixo
{
    internal class Piece 
    {
        Piece()
        {
            Rectangle rec = new Rectangle
                {
                    Width = Consts.PieceSize,
                    Height = Consts.PieceSize,
                    Fill = System.Windows.Media.Brushes.Brown  
            };
        }


        /* the xy parameters are relative to a grid!*/
        public void draw(int x,int y,System.Windows.Controls.Canvas GameArea)
        {
            // there is a problem that the peice oblect cant accsess/change the
            // GameArea object because it was passed as a reference witch means
            // that it will have to draw the stuff in the MainWindow.xaml.cs.
            GameArea.Children.Add(this.rec);
            GameArea.SetTop(rec, x*80);
            GameArea.SetLeft(rec, y*80);
        }
    }
}
