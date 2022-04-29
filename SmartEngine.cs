/*
ways to improve the smart engine:
[ ]: add more threads to calculate. not possible currently
		 with the minimax algorithm because it has to compare everything
[ ]: use better algorithm then MiniMaxWithAlphaBeta
[x]: use MinMaxWithAlphaBeta.
[ ]: use the bitboard directly, without the easy api (GetPiece,GetValidDestinationPieces)
		will boost the performance.
*/
using System.Linq;
using System.Diagnostics;

namespace Quixo
{
    public class SmartEngine
    {
        private const int DepthLimit = 4;//the const is faster!, but maybe its worth it to make it a real variable
        private const int LosingLine = int.MinValue;
        private const int WinningLine = int.MaxValue;
		private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
		public Move GenerateMove(Board board)
        {
			watch.Reset();
			watch.Start();
			int bestValue = int.MinValue;
            Move generatedMove = null ;
            foreach (var source in board.GetValidSourcePieces())
            {
                foreach (var destination in board.GetValidDestinationPieces(source))
                {
                    var nextMoveBoard = ((Board)board.Clone());
                    nextMoveBoard.MovePiece(source, destination);
                    var possibleBestMove = this.MiniMaxWithAlphaBeta(nextMoveBoard, board.CurrentPlayer, false, 1, int.MinValue, int.MaxValue);
                    if (possibleBestMove > bestValue || (possibleBestMove >= bestValue && generatedMove == null))
                    {
                        bestValue = possibleBestMove;
                        generatedMove = new Move(board.CurrentPlayer, source, destination);
                    }

                }
            }
			watch.Stop();
			Trace.WriteLine($"Execution Time:{watch.ElapsedMilliseconds} ms");
			return generatedMove;
        }
		private int MiniMaxWithAlphaBeta(Board board, Player currentPlayer, bool isMaximizing, int depth, int alpha, int beta)
		{
            int evaluation;
            if (depth>=DepthLimit||board.WinningPlayer!=Player.None)
			{

				evaluation = this.Evaluate(board,currentPlayer);
				if(board.CurrentPlayer!=Player.None&&board.CurrentPlayer!=currentPlayer)
				{
					evaluation*=-1;
				}
			else if(board.CurrentPlayer==Player.None&&board.WinningPlayer!=currentPlayer)
				{
					evaluation*=-1;
				}
			}
			else
			{
				var nextEvaluation=0;
				foreach(var source in board.GetValidSourcePieces())
				{
					foreach(var destination in board.GetValidDestinationPieces(source))
					{
						//Trace.WriteLine(String.Format("{0}{1}{2}",source.ToString(), destination.ToString(),depth.ToString()));
						var nextMoveBoard = ((Board)board.Clone());
						nextMoveBoard.MovePiece(source,destination);
						var newDepth = depth;
						nextEvaluation = this.MiniMaxWithAlphaBeta(nextMoveBoard,currentPlayer,!isMaximizing,++newDepth,alpha,beta);
						if(alpha>beta)
						{
							break;
						}
						else if (isMaximizing==false&&nextEvaluation<beta)
						{
							beta = nextEvaluation;
						}
						else if (isMaximizing==true&&nextEvaluation>alpha)
						{
							alpha = nextEvaluation;
						}
					}
					if(alpha>beta)
					{
						break;
					}
				}
				evaluation = isMaximizing?alpha:beta;

			}
			return evaluation;
		}
        #region Evaluating function

		public int Evaluate(Board board, Player currentPlayer)
		{
			int evaluation;
			if(board.WinningPlayer!=Player.None)
			{
				if(board.WinningPlayer==currentPlayer)
				{
					evaluation = WinningLine;
				}
				else
				{
					evaluation = LosingLine;
				}
			}
			else
			{
				evaluation = this.Evaluate(board);
			}
			return evaluation;
		}

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
        #endregion
    }
}
