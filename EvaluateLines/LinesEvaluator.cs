namespace Quixo
{
    internal abstract class LinesEvaluator
    {
        protected int UpdateContinuation(int currentContinuationValue) => (currentContinuationValue ^ 2) * 4;
        protected const int LosingLine = int.MinValue;
        protected const int WinningLine = int.MaxValue;
       abstract public int EvaluateLines(Board board, int evaluation);
    }
}
