using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Quixo
{
    /// <summary>
    /// handles everything about the game itself(logic)
    /// </summary>
    ///will inovke an event on every impotent change
    ///<see cref="event"/>
    public class Board:INotifyPropertyChanged
    {
        #region error strings
        private const string ErrorIdenticalPiece = "The source and destination locations are the same.";
        private const string ErrorInternalPiece = "Only the outer pieces can be moved.";
        private const string ErrorInvalidSourcePiece = "The player {0} cannot move the piece at position {1}.";
        private const string ErrorInvalidDestinationPosition = "The player {0} cannot move a piece to position {1}.";
        private const string ErrorWinner = "The game has been won by {0} - no more moves can be made.";
        #endregion
        #region delegates
        /// <summary>
        /// those are <see cerf ref="delegate"/> that will be use in the moving of the pieces.
        /// </summary>
        private delegate void AdjustLoopOperator(ref int sweep);
        private delegate bool CheckLoopOperator(int sweep, int checkPoint);
        private delegate int NextPieceOperator(int position);
        #endregion
        #region variables
        public const int Dimension = 5;
        private const int BitBoardOstart = Board.Dimension * Board.Dimension + 14;
        private const int BitBoardXStart = 7;
        private const int BitBoardOend = (Board.Dimension * Board.Dimension) * 2 + 14;
        private const int BitBoardXend = Board.Dimension * Board.Dimension + 7;
        private Player winningPlayer = Player.None;
        public Player WinningPlayer
        {
            get { return winningPlayer; }
            private set { if (winningPlayer != value)
                {
                    winningPlayer = value;
                    NotifyPropertyChanged();
                }
                }
        }
        private Player currentPlayer = Player.X;
        public Player CurrentPlayer
        {
        get { return currentPlayer; }
        private set { if (currentPlayer != value)
            {
            currentPlayer = value;
            NotifyPropertyChanged();
            }
        }
        }
        private ulong pieces;
        private MoveCollection? moveHistory = new MoveCollection();
        #endregion
        #region events declaration
        public event PropertyChangedEventHandler? PropertyChanged;
        public event Action Updated;
        public event Action<Move> MoveMade;
        #endregion
        /// <summary>
        /// used to notify UI binding targets that the value of a property has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName/*this is a runtime compiler parameter*/] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// constructor of the board,
        /// after the board is created, there is no need to call the <see cref="Reset"/> method. 
        /// </summary>
        /// <returns></returns>
        public Board() : base()
        {
            this.Reset();
        }
        public void Reset()
        {
            this.CurrentPlayer = Player.X;
            this.WinningPlayer = Player.None;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            moveHistory.Clear();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            this.pieces = 0;
        }
        /// <summary>
        /// returns the piece at the given position  
        /// returns a <see cerf="byte"/>, not a <see cerf="int"/>
        /// </summary>
        public byte GetDimension()
        {
            return (byte)Dimension;
        }
        /// <summary>
        /// verifies if the given position is valid
        /// and returns the valid destination position 
        /// </summary>
        /// <param name="source">the source <see cerf="Point"\> that was chosen</param>
        /// <returns>only the valid destinations of the source piece</returns>
        public List<Point> GetValidDestinationPieces(Point source)
        {
            var points = new List<Point>();

            if (IsOuterPiece(source) && this.CanCurrentPlayerUseSource(source))
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
        /// <summary>
        /// checks for valid source pieces and returns a lisf of them.
        /// </summary>
        /// <returns>a list of <see cerf="Point"/> that are valid source pieces</returns>
        public List<Point> GetValidSourcePieces()
        {
            var points = new List<Point>();

            for (var x = 1; x < Dimension; x++)
            {
                if (this.GetPiece(x, 0) == this.CurrentPlayer ||
                     this.GetPiece(x, 0) == Player.None)
                {
                    points.Add(new Point(x, 0));
                }
            }

            for (var y = 1; y < Dimension; y++)
            {
                if (this.GetPiece(Dimension - 1, y) == this.CurrentPlayer ||
                     this.GetPiece(Dimension - 1, y) == Player.None)
                {
                    points.Add(new Point(Dimension - 1, y));
                }
            }

            for (var x = Dimension - 2; x >= 0; x--)
            {
                if (this.GetPiece(x, Dimension - 1) == this.CurrentPlayer ||
                     this.GetPiece(x, Dimension - 1) == Player.None)
                {
                    points.Add(new Point(x, Dimension - 1));
                }
            }

            for (var y = Dimension - 2; y >= 0; y--)
            {
                if (this.GetPiece(0, y) == this.CurrentPlayer ||
                     this.GetPiece(0, y) == Player.None)
                {
                    points.Add(new Point(0, y));
                }
            }

            return points;
        }
        /// <summary>
        ///gets the piece type <see cerf="Player"/> at the given position 
        /// </summary>
        /// <param name="position"> <see cerf="Point"\></param>
        public Player GetPiece( Point position)
        {
            if (position.X < 0 || position.X > (Board.Dimension) ||
                position.Y < 0 || position.Y > (Board.Dimension))
            {
                throw new IndexOutOfRangeException($"Point {position.ToString()} is out of range.");
            }
            int shiftOut = this.GetShiftOut(position.X, position.Y);
            /*
                one of Characteristics of the bitBoard is that only one player can be
                standing on one tile, so there is no need to check if one is on and the other one is off
                because the second one must be off if the first one is on.
            */
            if ((this.pieces & (1UL << shiftOut)) == (1UL << shiftOut))//checking the X board.
            {
                return Player.X;
            }
            else if ((this.pieces & (1UL << (shiftOut + 32))) == (1UL << (shiftOut + 32)))//checking the O board.
            {
                return Player.O;
            }
            return (Player)Player.None;
        }
        /// <summary>
        ///there is a need to shift the mask bit to the right position,
        /// because of that, the shiftOut is calculated.
        /// every y coordinate is jumps of 5,
        /// and every x coordinate is an addition of 5.    
        private int GetShiftOut(int x, int y)
        {
            return BitBoardXStart + y * 5 + x;
            //the seven has no real use, but it is there to make the code more readable.
        }
        /// <summary>
        /// to get the x,y position of the given shiftOut position
        /// there is a need to reverse the shiftOut calculation.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Point GetReverseShiftOut(int position)
        {
            position -= BitBoardXStart;
            var x = position % 5;
            return new Point(x, (position-x)/5);

        }
        /// <summary>
        ///this function is used by the test engine to skip the player turn. 
        /// </summary>
        //  <param name="newOne"> replacement board</param>
        public void SetPiecesForTesting(ulong newOne)
        {
            this.pieces = newOne;
        }
        public void SetPiece(Point dest, Player newValue)
        {
            int shiftOut = this.GetShiftOut(dest.X, dest.Y);
            if (newValue == Player.X)
            {//there is a need to delete the record from the O bitborad
                this.pieces &= ~(1UL << (shiftOut + 32));
                this.pieces |= 1UL << shiftOut;
            }
            else if (newValue == Player.O)
            {//there is a need to delete the record from the X bitboard

                this.pieces &= ~(1UL << shiftOut);
                this.pieces |= 1UL << (shiftOut + 32);
            }
            else if (newValue == Player.None)
            {//delete from both bitboards

                this.pieces &= ~(1UL << shiftOut);
                this.pieces &= ~(1UL << (shiftOut + 32));
            }
            return;
        }
        /// <summary>
        ///a warper function for convenience.
        /// </summary>
        public void SetPiece(int x,int y,Player newValue)
        {
            SetPiece(new Point(x,y),newValue); 
        }
        /// <summary>
        ///function to provide the end point of the shift. 
        /// </summary>
        private int GetEndPoint(Point source, Point destination) =>
        source.X == destination.X ? destination.Y : destination.X;
        private bool CanCurrentPlayerUseSource(Point source)
        {
            var pieceState = this.GetPiece(source);

            return ((this.CurrentPlayer == Player.X && (pieceState == Player.X || pieceState == Player.None)) ||
                 (this.CurrentPlayer == Player.O && (pieceState == Player.O || pieceState == Player.None)));
        }
        /// <summary>
        ///check if the move is legal.
        ///  
        /// if it`s not throw appropriate exception
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void CheckPieces(Point source, Point destination)
        {
            if (source.Equals(destination) == true)
            {
                throw new InvalidMoveException(ErrorIdenticalPiece);
            }

            if (IsOuterPiece(source) == false || IsOuterPiece(destination) == false)
            {
                throw new InvalidMoveException(ErrorInternalPiece);
            }

            if (this.CanCurrentPlayerUseSource(source) == false)
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidSourcePiece, this.CurrentPlayer.ToString(), source.ToString()));
            }

            if (source.X != destination.X && source.Y != destination.Y)
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidDestinationPosition, this.CurrentPlayer.ToString(), destination.ToString()));
            }

            var endPoint = this.GetEndPoint(source, destination);

            if (endPoint != 0 && endPoint != (Dimension - 1))
            {
                throw new InvalidMoveException(
                     string.Format(ErrorInvalidDestinationPosition, this.CurrentPlayer.ToString(), destination.ToString()));
            }
        }
        /// <summary>
        /// checks if <param name="position"> is an outer piece.
        /// </summary>
        /// <returns>a boolean value</returns>
        private static bool IsOuterPiece(Point position) =>
            position.X != 0 || position.X != (Dimension - 1) ||
                 position.Y != 0 || position.Y != (Dimension - 1);

        /// <summary>
        ///wrapper function for convenience.
        /// will convert regular xy cords to point 
        /// object and send it to the right place.
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Player GetPiece(int x, int y)
        {
            return GetPiece(new Point(x,y));
        }
        private bool IsLessThan(int sweep, int checkPoint) => sweep < checkPoint;
        private bool IsGreaterThan(int sweep, int checkPoint) => sweep > checkPoint;
        private void Increment(ref int sweep) => sweep++;
        private void Decrement(ref int sweep) => sweep--;
        private int NextPieceBack(int position) => --position;
        private int NextPieceForward(int position) => ++position;
        
        /// <summary>
        /// function to wrap every thing that is needed to make a move with two points.
        /// will handle everything from checking if the move is legal 
        /// to actually making the move. 
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void MovePiece(Point source, Point destination)
        {
            var currentBoard = (Board)this.Clone();

            try
            {
                if (this.WinningPlayer != Player.None)//there is a winner already
                {
                    throw new InvalidMoveException(string.Format(ErrorWinner, this.WinningPlayer.ToString()));
                }

                this.CheckPieces(source, destination);
                this.UpdateBoard(source, destination);
                this.CheckWinningLines();

                moveHistory?.Add(new Move(this.CurrentPlayer, source, destination));

                this.CurrentPlayer = this.WinningPlayer != Player.None ? Player.None :
                    this.CurrentPlayer == Player.X ? Player.O : Player.X;
            }
            catch (InvalidMoveException)
            {
                this.CurrentPlayer = currentBoard.CurrentPlayer;
                this.WinningPlayer = currentBoard.WinningPlayer;

                for (var x = 0; x < Board.Dimension; x++)
                {
                    for (var y = 0; y < Board.Dimension; y++)
                    {
                        this.SetPiece(x, y, currentBoard.GetPiece(x, y));
                    }
                }

                throw;
            }
            Updated?.Invoke();
            MoveMade?.Invoke(new Move(this.CurrentPlayer, source, destination));
        }
        /// <summary>
        ///update the literal board with the new move.
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void UpdateBoard(Point source, Point destination)
        {
        /*
         * 1.the function updates only the bit-board! still there is a need to
         *   update the graphics in a different place.

         * 2.there is still a need to understand this to the full.
        */
            var newValue = this.CurrentPlayer;
            var isXFixed = source.X == destination.X;
            var fixedValue = (source.X == destination.X) ? source.X : source.Y;

            AdjustLoopOperator loopOp;
            CheckLoopOperator checkOp;
            NextPieceOperator nextPieceOp;

            if (source.Y > destination.Y || source.X > destination.X)
            {
                loopOp = new AdjustLoopOperator(this.Decrement);
                checkOp = new CheckLoopOperator(this.IsGreaterThan);
                nextPieceOp = new NextPieceOperator(this.NextPieceBack);
            }
            else
            {
                loopOp = new AdjustLoopOperator(this.Increment);
                checkOp = new CheckLoopOperator(this.IsLessThan);
                nextPieceOp = new NextPieceOperator(this.NextPieceForward);
            }

            var startPoint = isXFixed ? source.Y : source.X;
            var endPoint = isXFixed ? destination.Y : destination.X;

            for (var sweep = startPoint; checkOp(sweep, endPoint); loopOp(ref sweep))
            {
                if (isXFixed == true)
                {
                    this.SetPiece(fixedValue, sweep,
                        this.GetPiece(fixedValue, nextPieceOp(sweep)));
                }
                else
                {
                    this.SetPiece(sweep, fixedValue,
                        this.GetPiece(nextPieceOp(sweep), fixedValue));
                }
            }

            this.SetPiece(destination.X, destination.Y, newValue);
        }
        /// <summary>
        ///check if there is a winner. 
        /// </summary>
        private void CheckWinningLines()
        {
            var lines = new WinningLines(this);

            if ((this.CurrentPlayer == Player.X && lines.OCount > 0) ||
                 (this.CurrentPlayer == Player.O && lines.OCount > 0 && lines.XCount == 0))
            {
                this.WinningPlayer = Player.O;
            }
            else if ((this.CurrentPlayer == Player.O && lines.XCount > 0) ||
                 (this.CurrentPlayer == Player.X && lines.XCount > 0 && lines.OCount == 0))
            {
                this.WinningPlayer = Player.X;
            }
            else
            {
                this.WinningPlayer = Player.None;
            }
        }
        public MoveCollection Moves => this.moveHistory;

        /// <summary>
        /// extract an list of pieces from bitBoard
        /// </summary>
        public List<Piece> ExtractPiecesFromBitBoard()
        {
            List<Piece> pieces = new List<Piece>();
            for (int i = BitBoardXStart; i < BitBoardXend; i++)//for X
            {

                if ((this.pieces & 1UL << i) == (1UL << i))
                {
                    pieces.Add(new Piece(GetReverseShiftOut(i), Player.X));
                }
            }

            for (int i = BitBoardOstart; i < BitBoardOend; i++)//for y
            {

                if ((this.pieces & 1UL << i) == (1UL << i))
                {
                    pieces.Add(new Piece(GetReverseShiftOut(i- BitBoardOstart+BitBoardXStart), Player.O));
                }
            }
            return pieces;
        }
        /// <summary>
        /// clone the board, there is no action to be done after this,
        /// all is taken care of.
        /// </summary>
        /// <returns>
        /// a clone of the current board
        /// </returns>
        public Board Clone()
        {
            return new Board
            {
                CurrentPlayer = this.CurrentPlayer,
                WinningPlayer = this.WinningPlayer,
                moveHistory = moveHistory?.Clone() as MoveCollection,//if this function is reached
                pieces = this.pieces
            };
        }
        /// <summary>
        /// class to store and calculate a winner
        /// </summary>
        private sealed class WinningLines
        {//must be more efficient. probably will use preknown bitboards
         // that i know, there are only 12*2
            private const string ErrorInvalidLineCount = "The line count should be {0} but it was {1}.";
            private static readonly int WinningLineCount = (Board.Dimension * 2) + 2;

            private readonly Board board ;
            private int blankCount = 0;
            private int xCount = 0;
            private int oCount = 0;

        private readonly ulong[] xWon =  {
               //<unused><------x--------------->unused><------y----------------->
                0b0000000000000000000000000000000000000000000000000000111110000000,
                0b0000000000000000000000000000000000000000000000011111000000000000,
                0b0000000000000000000000000000000000000000001111100000000000000000,
                0b0000000000000000000000000000000000000111110000000000000000000000,
                0b0000000000000000000000000000000011111000000000000000000000000000,
                0b0000000000000000000000000000000000001000010000100001000010000000,
                0b0000000000000000000000000000000000010000100001000010000100000000,
                0b0000000000000000000000000000000000100001000010000100001000000000,
                0b0000000000000000000000000000000001000010000100001000010000000000,
                0b0000000000000000000000000000000010000100001000010000100000000000,
                0b0000000000000000000000000000000010000010000010000010000010000000,
                0b0000000000000000000000000000000000001000100010001000100000000000,};

        private readonly ulong[] yWon ={
                0b0000000000000000000011111000000000000000000000000000000000000000,
                0b0000000000000001111100000000000000000000000000000000000000000000,
                0b0000000000111110000000000000000000000000000000000000000000000000,
                0b0000011111000000000000000000000000000000000000000000000000000000,
                0b1111100000000000000000000000000000000000000000000000000000000000,
                0b0000100001000010000100001000000000000000000000000000000000000000,
                0b0001000010000100001000010000000000000000000000000000000000000000,
                0b0010000100001000010000100000000000000000000000000000000000000000,
                0b0100001000010000100001000000000000000000000000000000000000000000,
                0b1000010000100001000010000000000000000000000000000000000000000000,
                0b1000001000001000001000001000000000000000000000000000000000000000,
                0b0000100010001000100010000000000000000000000000000000000000000000,};
            private WinningLines() : base() { }

            public WinningLines(Board board)
                : this()
            {
                this.board = board;
                this.CalculateWinners();
            }

            private void CalculateWinners()
            {
                //see if the board is equal to any of the winning lines from the xWon array
                for (int i = 0; i < WinningLineCount; i++)
                {
                    if ((this.xWon[i] & this.board.pieces)==this.xWon[i])
                    {
                        this.xCount++;
                        return;
                    }
                }
                //see if the board is equal to any of the winning lines from the yWon array
                for (int i = 0; i < WinningLineCount; i++)
                {
                    if ((this.yWon[i]  &this.board.pieces)==this.yWon[i])
                    {
                        this.oCount++;
                        return;
                    }
                }
            }



            public int NoneCount => this.blankCount;

            public int XCount => this.xCount;

            public int OCount => this.oCount;
        }

    }
}
