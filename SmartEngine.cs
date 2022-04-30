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
		public Move? GenerateMove(Board board)
        {
			watch.Reset();
			watch.Start();
			int bestValue = int.MinValue;
            Move? generatedMove = null ;
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
                }
            }
            return evaluation;
        }
        #endregion
    }
}
