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
        private const string ErrorIdenticalPiece = "The source and destination locations are the same.";
        private const string ErrorInternalPiece = "Only the outer pieces can be moved.";
        private const string ErrorInvalidSourcePiece = "The player {0} cannot move the piece at position {1}.";
        private const string ErrorInvalidDestinationPosition = "The player {0} cannot move a piece to position {1}.";
        private const string ErrorWinner = "The game has been won by {0} - no more moves can be made.";

        private delegate void AdjustLoopOperator(ref int sweep);
        private delegate bool CheckLoopOperator(int sweep, int checkPoint);
        private delegate int NextPieceOperator(int position);

        public const int Dimension = 5;
        private Player winningPlayer = Player.None;
        private Player currentPlayer = Player.X;
        private long pieces;
        private MoveCollection moveHistory = new MoveCollection();

        public Board() : base()
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
        public List<Point> GetValidDestinationPieces(Point source)
        {
            var points = new List<Point>();

            if (this.IsOuterPiece(source) && this.CanCurrentPlayerUseSource(source))
            {
                if (source.X == 0)
                {
                    points.Add(new Point(Board.Dimension - 1, source.Y));
                }
                else if (source.X == (Dimension - 1))
                {
                    points.Add(new Point(0, source.Y));
                }
                else
                {
                    points.Add(new Point(Dimension - 1, source.Y));
                    points.Add(new Point(0, source.Y));
                }

                if (source.Y == 0)
                {
                    points.Add(new Point(source.X, Dimension - 1));
                }
                else if (source.Y == (Dimension - 1))
                {
                    points.Add(new Point(source.X, 0));
                }
                else
                {
                    points.Add(new Point(source.X, Dimension - 1));
                    points.Add(new Point(source.X, 0));
                }
            }

            return points;
        }
        public List<Point> GetValidSourcePieces()
        {
            var points = new List<Point>();

            for (var x = 1; x < Dimension; x++)
            {
                if (this.GetPiece(x, 0) == this.currentPlayer ||
                     this.GetPiece(x, 0) == Player.None)
                {
                    points.Add(new Point(x, 0));
                }
            }

            for (var y = 1; y < Dimension; y++)
            {
                if (this.GetPiece(Dimension - 1, y) == this.currentPlayer ||
                     this.GetPiece(Dimension - 1, y) == Player.None)
                {
                    points.Add(new Point(Dimension - 1, y));
                }
            }

            for (var x = Dimension - 2; x >= 0; x--)
            {
                if (this.GetPiece(x, Dimension - 1) == this.currentPlayer ||
                     this.GetPiece(x, Dimension - 1) == Player.None)
                {
                    points.Add(new Point(x, Dimension - 1));
                }
            }

            for (var y = Dimension - 2; y >= 0; y--)
            {
                if (this.GetPiece(0, y) == this.currentPlayer ||
                     this.GetPiece(0, y) == Player.None)
                {
                    points.Add(new Point(0, y));
                }
            }

            return points;
        }
        public Player GetPiece( Point position)
        {
            if (position.X < 0 || position.X > (Board.Dimension) ||
                position.Y < 0 || position.Y > (Board.Dimension))
            {
                throw new IndexOutOfRangeException($"Point {position.ToString()} is out of range.");
            }

            int shiftOut = this.GetShiftOut(position.X, position.Y);
            if ((this.pieces & (1L << shiftOut)) == (1L << shiftOut))
            {
                return Player.X;
            }
            else if ((this.pieces & (1L >> (shiftOut + 32))) == (1L >> (shiftOut + 32)))
            {
                return Player.O;
            }
            return (Player)Player.None;
        }
        private int GetShiftOut(int x, int y)
        {
            return 7 + y * 5 + x;//need to correct the board/stay with it. in my description there is a divider of 7 emply bits between both of X,and O boards.
        }
        private Point GetReverseShiftOut(int position)
        {
            position -= 7;
            var x = position % 5;
            return new Point(x, (position-x)/5);

        }
        public void SetPiecesForTesting(long newOne)
        {
            this.pieces = newOne;
        }
        public void SetPiece(Point dest, Player newValue)
        {
            int shiftOut = this.GetShiftOut(dest.X, dest.Y);
            if (newValue == Player.X)
            {//there is a need to delete the record from the O bitborad
                this.pieces &= ~(1L << (shiftOut + 32));
                this.pieces |= 1L << shiftOut;
            }
            else if (newValue == Player.O)
            {//there is a need to delete the record from the X bitboard

                this.pieces &= ~(1L << shiftOut);
                this.pieces |= 1L << (shiftOut + 32);
            }
            else if (newValue == Player.None)
            {//delete from both bitboards

                this.pieces &= ~(1L << shiftOut);
                this.pieces &= ~(1L << (shiftOut + 32));
            }
            return;
        }
        public void SetPiece(int x,int y,Player newValue)
        {
            SetPiece(new Point(x,y),newValue); 
        }
        private int GetEndPoint(Point source, Point destination) =>
        source.X == destination.X ? destination.Y : destination.X;
        private bool CanCurrentPlayerUseSource(Point source)
        {
            var pieceState = this.GetPiece(source);

            return ((this.currentPlayer == Player.X && (pieceState == Player.X || pieceState == Player.None)) ||
                 (this.currentPlayer == Player.O && (pieceState == Player.O || pieceState == Player.None)));
        }
        private void CheckPieces(Point source, Point destination)
        {
            if (source.Equals(destination) == true)
            {
                throw new InvalidMoveException(ErrorIdenticalPiece);
            }

            if (this.IsOuterPiece(source) == false || this.IsOuterPiece(destination) == false)
            {
                throw new InvalidMoveException(ErrorInternalPiece);
            }

            if (this.CanCurrentPlayerUseSource(source) == false)
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidSourcePiece, this.currentPlayer.ToString(), source.ToString()));
            }

            if (source.X != destination.X && source.Y != destination.Y)
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidDestinationPosition, this.currentPlayer.ToString(), destination.ToString()));
            }

            var endPoint = this.GetEndPoint(source, destination);

            if (endPoint != 0 && endPoint != (Dimension - 1))
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidDestinationPosition, this.currentPlayer.ToString(), destination.ToString()));
            }
        }
        private bool IsOuterPiece(Point position) =>
            position.X != 0 || position.X != (Dimension - 1) ||
                 position.Y != 0 || position.Y != (Dimension - 1);
        private void movePiece(Point src ,Point dest)
        {
            var currentBoard = (Board)this.Clone();
            try
            {
                if (this.winningPlayer != Player.None)
                {//there is a winner
                    throw new InvalidMoveException(string.Format(ErrorWinner, this.winningPlayer.ToString()));
                }
                this.CheckPieces(src, dest);
                this.UpdateBoard(src, dest);
                this.CheckWinningLines();
                this.moveHistory.Add(new Move(this.currentPlayer, src, dest));
                //i have no idea how this is working
                this.currentPlayer = this.winningPlayer != Player.None ? Player.None :
                    this.currentPlayer == Player.X ? Player.O : Player.X;
            }
            catch (InvalidMoveException)
            {
                this.currentPlayer = currentBoard.currentPlayer;
                this.winningPlayer = currentBoard.winningPlayer;

                for (var x = 0; x < Board.Dimension; x++)
                {
                    for (var y = 0; y < Board.Dimension; y++)
                    {
                        this.SetPiece(x, y, currentBoard.GetPiece(x, y));
                    }
                }

                throw;
            }
        }
        private Player GetPiece(int x, int y)
        {
            return GetPiece(new Point(x,y));
        }

        private object GetPiece(int x, int y)
        {
            throw new NotImplementedException();
        }
        private bool IsLessThan(int sweep, int checkPoint) => sweep < checkPoint;
        private bool IsGreaterThan(int sweep, int checkPoint) => sweep > checkPoint;
        private void Increment(ref int sweep) => sweep++;
        private void Decrement(ref int sweep) => sweep--;
        private int NextPieceBack(int position) => --position;
        private int NextPieceForward(int position) => ++position;

        private void CheckWinningLines()
        {
            throw new NotImplementedException();
        }

        private void UpdateBoard(Point src, Point dest)
        {
            throw new NotImplementedException();
        }

        private Board Clone()
        {
            return new Board
            {
                currentPlayer = this.currentPlayer,
                winningPlayer = this.winningPlayer,
                moveHistory = this.moveHistory.Clone() as MoveCollection,
                pieces = this.pieces
            };
        }
        private sealed class WinningLines
        {//must be more efficient. probably will use preknown bitboards
         // that i know, there are only 12*2
            private const string ErrorInvalidLineCount = "The line count should be {0} but it was {1}.";
            private static readonly int WinningLineCount = (Board.Dimension * 2) + 2;

            private readonly Board board = null;
            private int blankCount = 0;
            private int xCount = 0;
            private int oCount = 0;

            private WinningLines() : base() { }

            public WinningLines(Board board)
                : this()
            {
                this.board = board;
                this.CalculateWinningCounts();
            }

            private void CalculateWinningCounts()
            {
                this.CalculateHorizontalWinners();
                this.CalculateVerticalWinners();
                this.CalculateDiagonalWinners();
                var winningLineCount = this.xCount + this.oCount + this.blankCount;

                if (winningLineCount != WinningLineCount)
                {
                    throw new InvalidOperationException(
                         string.Format(ErrorInvalidLineCount, WinningLineCount, winningLineCount));
                }
            }

            private void CalculateHorizontalWinners()
            {
                for (var y = 0; y < Board.Dimension; y++)
                {
                    var lineState = this.board.GetPiece(0, y);

                    for (var x = 1; x < Board.Dimension; x++)
                    {
                        var currentPiece = this.board.GetPiece(x, y);

                        if (currentPiece == Player.None || currentPiece != lineState)
                        {
                            lineState = Player.None;
                            break;
                        }
                    }

                    this.UpdatePlayerWinCount(lineState);
                }
            }

            private void CalculateVerticalWinners()
            {
                for (var x = 0; x < Board.Dimension; x++)
                {
                    var lineState = this.board.GetPiece(x, 0);

                    for (var y = 1; y < Board.Dimension; y++)
                    {
                        var currentPiece = this.board.GetPiece(x, y);

                        if (currentPiece == Player.None || currentPiece != lineState)
                        {
                            lineState = Player.None;
                            break;
                        }
                    }

                    this.UpdatePlayerWinCount(lineState);
                }
            }

            private void CalculateDiagonalWinners()
            {
                var lineState = this.board.GetPiece(0, 0);

                for (var x = 0; x < Board.Dimension; x++)
                {
                    var currentPiece = this.board.GetPiece(x, x);

                    if (currentPiece == Player.None || currentPiece != lineState)
                    {
                        lineState = Player.None;
                        break;
                    }
                }

                this.UpdatePlayerWinCount(lineState);

                lineState = this.board.GetPiece(0, Board.Dimension - 1);

                for (var x = 0; x < Board.Dimension; x++)
                {
                    var currentPiece = this.board.GetPiece(x, Board.Dimension - 1 - x);

                    if (currentPiece == Player.None || currentPiece != lineState)
                    {
                        lineState = Player.None;
                        break;
                    }
                }

                this.UpdatePlayerWinCount(lineState);
            }

            private void UpdatePlayerWinCount(Player lineWinner)
            {
                if (lineWinner == Player.X)
                {
                    this.xCount++;
                }
                else if (lineWinner == Player.O)
                {
                    this.oCount++;
                }
                else
                {
                    this.blankCount++;
                }
            }

            public int NoneCount => this.blankCount;

            public int XCount => this.xCount;

            public int OCount => this.oCount;
        }
    }
}
