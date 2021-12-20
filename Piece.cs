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
    }
}
