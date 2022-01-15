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
		public Player GetPiece(Point position)
		{
			if (position.X < 0 || position.X > (Board.Dimension) ||
				position.Y < 0 || position.Y > (Board.Dimension))
			{
				throw new IndexOutOfRangeException($"Point {position.ToString()} is out of range.");
			}

			int shiftOut = this.GetShiftOut(position.X, position.Y);
			if ((this.pieces & (1L << shiftOut)) ==(1L << shiftOut))
			{
				return Player.X;
			}
			else if ((this.pieces & (1L >> (shiftOut + 32))) == (1L>>(shiftOut + 32)))
                {
				return Player.O;
            }
			return (Player)Player.None;
		}

	}
}
