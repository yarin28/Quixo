using System;
using System.Collections.Generic;
using System.Drawing;
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
		public Board():base()
        {
			this.Reset();
        }
		private void Reset()
		{
			this.currentPlayer = Player.X;
			this.winningPlayer = Player.None;
			this.moveHistory.Clear();
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

        private int GetShiftOut(int x, int y)
        {
			return 7 + y * 5 + x;//need to correct the board/stay with it. in my description there is a divider of 7 emply bits between both of X,and O boards.
        }
		public void SetPiecesForTesting(long newOne)
        {
			this.pieces = newOne;
        }
		public void SetPiece(Point dest,Player newValue)
        {
			int shiftOut = this.GetShiftOut(dest.X, dest.Y);
			if (newValue==Player.X)
            {//there is a need to delete the record from the O bitborad
			this.pieces &= ~(1L << (shiftOut+32));
			this.pieces |= 1L << shiftOut;
            }
			else if(newValue==Player.O)
            {//there is a need to delete the record from the X bitboard

			this.pieces &= ~(1L << shiftOut);
			this.pieces |= 1L << (shiftOut+32);
            }
			else if(newValue==Player.None)
            {//delete from both bitboards

			this.pieces &= ~(1L << shiftOut);
			this.pieces &= ~(1L << (shiftOut+32));
            }
			return;
        }
    }
}
