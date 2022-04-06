using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Quixo
{
    public class SmartEngine
    {
        private const int DepthLimit = 4;
        private const int LosingLine = int.MinValue;
        private const int WinningLine = int.MaxValue;
		private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        public int Evaluate(Board board)
        {
			     var evaluation = 0;
				 //NOTE: the game is won.
				 if(board.WinningPlayer!=Player.None)
				 {
					 if(board.Moves.Count>0)
					 {
						 var lastMove = board.Moves.Last();
						 if(lastMove.Player==board.WinningPlayer)
						 {
							 evaluation = WinningLine;
						 }
						 else
						 {
							 evaluation = LosingLine;
						 }
					 }
				 }
				 else//NOTE: the game is in progress
				 {
					evaluation = this.EvaluateHorizontalLines(board,evaluation);
					if(evaluation!=LosingLine&&evaluation!=WinningLine)
					{
						evaluation = this.EvaluateVerticalLines(board,evaluation);
					}
					if(evaluation!=LosingLine&&evaluation!=WinningLine)
					{
						evaluation = this.EvaluateDiagonalLines(board,evaluation);
					}
				 }
				 return evaluation;
        }
        //the Evaluate function must reword 2 things
		//1- the number of pieces of the current player
		//2- the connections between the pieces of the current player
		//TODO: this will probably have to change.
	private int UpdateContinuation(int currentContinuationValue) =>
			(currentContinuationValue ^ 2) * 4;

		private int EvaluateHorizontalLines(Board board, int evaluation)
		{
			var horizontalEvaluation = evaluation;
			bool hasWinningLine = false, hasLosingLine = false;

			for(var y = 0; y < Board.Dimension; y++)
			{
				var lineState = board.GetPiece(0, y);

				if(lineState == board.CurrentPlayer)
				{
					horizontalEvaluation++;
				}
				else if(lineState != Player.None)
				{
					horizontalEvaluation--;
				}

				var continuationFactor = 1;

				for(var x = 1; x < Board.Dimension; x++)
				{
					var currentPiece = board.GetPiece(x, y);

					if(currentPiece == board.CurrentPlayer)
					{
						horizontalEvaluation++;
					}
					else if(currentPiece != Player.None)
					{
						horizontalEvaluation--;
					}

					if(currentPiece == board.GetPiece(x - 1, y))
					{
						continuationFactor = this.UpdateContinuation(continuationFactor);

						if(currentPiece == board.CurrentPlayer)
						{
							horizontalEvaluation += continuationFactor;
						}
						else if(currentPiece != Player.None)
						{
							horizontalEvaluation -= continuationFactor;
						}
					}
					else
					{
						lineState = Player.None;
						continuationFactor = 1;
					}
				}

				if(lineState == board.CurrentPlayer)
				{
					hasWinningLine = true;
					break;
				}
				else if(lineState != Player.None)
				{
					hasLosingLine = true;
					break;
				}
			}

			if(hasWinningLine == true && hasLosingLine == false)
			{
				horizontalEvaluation = WinningLine;
			}
			else if(hasLosingLine == true)
			{
				horizontalEvaluation = LosingLine;
			}

			return horizontalEvaluation;
		}

		private int EvaluateVerticalLines(Board board, int evaluation)
		{
			var verticalEvaluation = evaluation;
			bool hasWinningLine = false, hasLosingLine = false;

			for(var x = 0; x < Board.Dimension; x++)
			{
				var lineState = board.GetPiece(x, 0);

				if(lineState == board.CurrentPlayer)
				{
					verticalEvaluation++;
				}
				else if(lineState != Player.None)
				{
					verticalEvaluation--;
				}

				var continuationFactor = 1;

				for(var y = 1; y < Board.Dimension; y++)
				{
					var currentPiece = board.GetPiece(x, y);

					if(currentPiece == board.CurrentPlayer)
					{
						verticalEvaluation++;
					}
					else if(currentPiece != Player.None)
					{
						verticalEvaluation--;
					}

					if(currentPiece == board.GetPiece(x, y - 1))
					{
						continuationFactor = this.UpdateContinuation(continuationFactor);

						if(currentPiece == board.CurrentPlayer)
						{
							verticalEvaluation += continuationFactor;
						}
						else if(currentPiece != Player.None)
						{
							verticalEvaluation -= continuationFactor;
						}
					}
					else
					{
						lineState = Player.None;
						continuationFactor = 1;
					}
				}

				if(lineState == board.CurrentPlayer)
				{
					hasWinningLine = true;
					break;
				}
				else if(lineState != Player.None)
				{
					hasLosingLine = true;
					break;
				}
			}

			if(hasWinningLine == true && hasLosingLine == false)
			{
				verticalEvaluation = WinningLine;
			}
			else if(hasLosingLine == true)
			{
				verticalEvaluation = LosingLine;
			}

			return verticalEvaluation;
		}

		private int EvaluateDiagonalLines(Board board, int evaluation)
		{
			var diagonalEvaluation = evaluation;

			var lineState = board.GetPiece(0, 0);

			if(lineState == board.CurrentPlayer)
			{
				diagonalEvaluation++;
			}
			else if(lineState != Player.None)
			{
				diagonalEvaluation--;
			}

			var continuationFactor = 1;

			for(var x = 1; x < Board.Dimension; x++)
			{
				var currentPiece = board.GetPiece(x, x);

				if(currentPiece == board.CurrentPlayer)
				{
					diagonalEvaluation++;
				}
				else if(currentPiece != Player.None)
				{
					diagonalEvaluation--;
				}

				if(currentPiece == board.GetPiece(x - 1, x - 1))
				{
					continuationFactor = this.UpdateContinuation(continuationFactor);

					if(currentPiece == board.CurrentPlayer)
					{
						diagonalEvaluation += continuationFactor;
					}
					else if(currentPiece != Player.None)
					{
						diagonalEvaluation -= continuationFactor;
					}
				}
				else
				{
					lineState = Player.None;
					continuationFactor = 1;
				}
			}

			if(lineState != Player.None && lineState != board.CurrentPlayer)
			{
				diagonalEvaluation = LosingLine;
			}
			else
			{
				if(lineState == board.CurrentPlayer)
				{
					diagonalEvaluation = WinningLine;
				}
				else
				{
					lineState = board.GetPiece(0, Board.Dimension - 1);

					if(lineState == board.CurrentPlayer)
					{
						diagonalEvaluation++;
					}
					else if(lineState != Player.None)
					{
						diagonalEvaluation--;
					}

					for(var x = 1; x < Board.Dimension; x++)
					{
						var currentPiece = board.GetPiece(x, Board.Dimension - 1 - x);

						if(currentPiece == board.CurrentPlayer)
						{
							diagonalEvaluation++;
						}
						else if(currentPiece != Player.None)
						{
							diagonalEvaluation--;
						}

						if(currentPiece == board.GetPiece(x - 1, Board.Dimension - x))
						{
							continuationFactor = this.UpdateContinuation(continuationFactor);

							if(currentPiece == board.CurrentPlayer)
							{
								diagonalEvaluation += continuationFactor;
							}
							else if(currentPiece != Player.None)
							{
								diagonalEvaluation -= continuationFactor;
							}
						}
						else
						{
							lineState = Player.None;
							continuationFactor = 1;
						}
					}

					if(lineState == board.CurrentPlayer)
					{
						diagonalEvaluation = WinningLine;
					}
					else if(lineState != Player.None)
					{
						diagonalEvaluation = LosingLine;
					}
				}
			}

			return diagonalEvaluation;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="currentPlayer"></param>
        /// <returns>
        /// returns value base on number of consecrative tiles on each row, column, and both of the diagonal for <see cref="Board" /> board <see cref="Player"/> currentPlayer
        /// </returns>
        private static int CountConsecutive(Board board, Player currentPlayer)
        {
            //TODO: this is bad i have no idea what is happening here should use bit board for this!
            int r = 0;
            int nr = 0;
            int mod = 2;
            int nMod = 1;
            //horizontal
            for (int y = 0; y < 5; y++)
            {
                int inRow = 0;
                int nInRow = 0;
                for (int x = 0; x < 5; x++)
                {
                    Player piecePlayer = board.GetPiece(new Point(x, y));
                    if (Player.None == piecePlayer)
                    {
                        if (piecePlayer == currentPlayer)
                        {
                            inRow++;
                        }
                        else
                        {
                            nInRow++; ;
                        }
                    }
                    r += inRow * inRow;
                    nr += nInRow * nInRow;
                }
            }

            for (int x = 0; x < 5; x++)
            {
                int inCol = 0;
                int nInCol = 0;
                for (int y = 0; y < 5; y++)
                {
                    Player piecePlayer = board.GetPiece(new Point(x, y));
                    if (Player.None == piecePlayer)
                    {
                        if (piecePlayer == currentPlayer)
                        {
                            inCol++;
                        }
                        else
                        {
                            nInCol++;
                        }
                    }
                    r += inCol * inCol;
                    nr += nInCol * nInCol;
                }
            }

            return r - nr;
        }
    }
}
