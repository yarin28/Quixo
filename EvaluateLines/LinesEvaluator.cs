using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quixo
{
    internal abstract class LinesEvaluator
    {
        protected int UpdateContinuation(int currentContinuationValue) =>
           
			(currentContinuationValue ^ 2) * 4;
        protected const int LosingLine = int.MinValue;
        protected const int WinningLine = int.MaxValue;
