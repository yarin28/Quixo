using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quixo
{
    //public sealed  class Board
    public class Board
    {
		public const int Dimension = 5;

		private Player winningPlayer = Player.None;
		private Player currentPlayer = Player.X;
		private long pieces;
		private MoveCollection moveHistory = new MoveCollection();
		public Board()
        {
			this.pieces = 0;

        }
		public byte GetDimension()
        {
			return (byte)Dimension;
        }
	}
}
