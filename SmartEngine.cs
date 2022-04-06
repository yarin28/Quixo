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
    }
}
