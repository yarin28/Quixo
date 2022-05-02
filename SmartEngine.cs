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
using Quixo.EvaluateLines;
using System;

namespace Quixo
{
    public class SmartEngine
    {
        private  int depthLimit = 4;//the const is faster!, but maybe its worth it to make it a real variable
        private const int LosingLine = int.MinValue;
        private const int WinningLine = int.MaxValue;
        public void SetDepthLimit(int depthLimit)
        {
            this.depthLimit = depthLimit;
        }
        static readonly LinesEvaluator[] linesEvaluators = new LinesEvaluator[] {
            new HorizontalLinesEvaluator(),
            new VerticalLinesEvaluator(),
            new DiagonalLinesEvaluator()};
        public Move GenerateMove(Board board)
        {
                    var (generatedMove, evaluation) = MiniMaxWithAlphaBeta(board, board.CurrentPlayer, true, 1,  int.MinValue, int.MaxValue);

            if (generatedMove == null)
                throw new NullReferenceException("generatedMove was null");
            return generatedMove;
        }
        private (Move?,int) MiniMaxWithAlphaBeta(Board board, Player currentPlayer, bool isMaximizing, int depth,  int  alpha, int beta)
        {
            int evaluation;
            Move? nextMove = null;
            if (depth >= depthLimit || board.WinningPlayer != Player.None)
            {

                evaluation = this.Evaluate(board, currentPlayer);
                if (board.CurrentPlayer != Player.None && board.CurrentPlayer != currentPlayer)
                    evaluation *= -1;
                else if (board.CurrentPlayer == Player.None && board.WinningPlayer != currentPlayer)
                    evaluation *= -1;
            }
            else
            {
                foreach (var source in board.GetValidSourcePieces())
                {
                    foreach (var destination in board.GetValidDestinationPieces(source))
                    {
                        var nextMoveBoard = board.Clone();
                        nextMoveBoard.MovePiece(source, destination);
                        var newDepth = depth;
                        var (_, nextEvaluation) = this.MiniMaxWithAlphaBeta(nextMoveBoard, currentPlayer, !isMaximizing, ++newDepth, alpha, beta);
                        if (alpha > beta)
                            break;
                        else if (isMaximizing == false && nextEvaluation < beta)
                        {
                            nextMove = new(board.CurrentPlayer, source, destination);
                            beta = nextEvaluation;
                        }
                        else if (isMaximizing == true && nextEvaluation > alpha)
                        {
                            nextMove = new(board.CurrentPlayer, source, destination);
                            alpha = nextEvaluation;
                        }
                    }
                    if (alpha > beta)
                        break;
                }
                evaluation = isMaximizing ? alpha : beta;

            }
            //NOTE: if the nextMove is null someone has won.
            return (nextMove, evaluation);
        }
        #region Evaluating function

        public int Evaluate(Board board, Player currentPlayer)
        {
            int evaluation;
            if (board.WinningPlayer != Player.None)
            {
                if (board.WinningPlayer == currentPlayer)
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
            if (board.WinningPlayer != Player.None)
            {
                if (board.Moves.Count > 0)
                {
                    var lastMove = board.Moves.Last();
                    if (lastMove.Player == board.WinningPlayer)
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
                foreach (var evaluatingFunction in linesEvaluators)
                {
                    evaluation = +evaluatingFunction.EvaluateLines(board, evaluation);
                    if (evaluation == LosingLine || evaluation == WinningLine)
                        break;
                }
            }
            return evaluation;
        }
        #endregion
    }
}
