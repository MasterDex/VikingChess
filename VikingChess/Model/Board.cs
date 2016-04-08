using System;
using System.Linq;

namespace VikingChess.Model
{
    /// <summary>
    /// The <see cref="Board"/> class represents the play board for the game.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// The array holding the board data
        /// </summary>
        private Square[,] board;
        /// <summary>
        /// The size of the board.
        /// </summary>
        private int size;
        /// <summary>
        /// The player who moves next
        /// </summary>
        private Player currentPlayer;
        /// <summary>
        /// The type of <see cref="Game"/> this is.
        /// </summary>
        private String gameType = null;
        /// <summary>
        /// The number of black pieces on this board.
        /// </summary>
        private int blackPieces = 0;
        /// <summary>
        /// The number of white pieces on this board.
        /// </summary>
        private int whitePieces = 0;

        /// <summary>
        /// Constructor. Makes a new board object of the specified <see cref="size"/>
        /// </summary>
        /// <param name="size">The size of the <see cref="Board"/>.</param>
        /// <param name="gameType"><see cref="gameType"/></param>
        public Board(int size, String gameType)
        {
            this.size = size + 2;
            board = new Square[this.size, this.size];
            this.gameType = gameType;
            InitializeBoard();
            SetPieces();
            SetSpecialSquares();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="prevBoard">The <see cref="Board"/> to copy.</param>
        public Board(Board prevBoard)
        {
            this.size = prevBoard.GetSize();
            this.board = new Square[this.size, this.size];
            this.currentPlayer = new Player(prevBoard.getCurrentPlayer());
            this.gameType = prevBoard.gameType;
            InitializeBoard();
            for(int i=1; i < size-1; ++i)
            {
                for(int j=1; j < size-1; ++j)
                {
                    if(prevBoard.GetSquare(i, j).getPiece() != null)
                    {
                        board[i, j].setPiece(new Piece(prevBoard.GetSquare(i, j).getPiece()));
                    }
                    else
                    {
                        board[i, j].setPiece(null);
                    }
                }
            }
            blackPieces = prevBoard.countPieces(Enums.Color.BLACK);
            whitePieces = prevBoard.countPieces(Enums.Color.WHITE);
            SetSpecialSquares();
        }

        /// <summary>
        /// Decrements the <see cref="blackPieces"/> on this board.
        /// </summary>
        public void decrementBlackCount()
        {
            --blackPieces;
        }

        /// <summary>
        /// Decrements the <see cref="whitePieces"/> on this board.
        /// </summary>
        public void decrementWhiteCount()
        {
            --whitePieces;
        }
        
        /// <summary>
        /// Creates new <see cref="Square"/> objects at the appropriate location in the board array and defines the neighbours
        /// for each square.
        /// </summary>
        public void InitializeBoard()
        {
            Enums.Rank[] ranks = (Enums.Rank[])Enum.GetValues(typeof(Enums.Rank));
            Enums.File[] files = (Enums.File[])Enum.GetValues(typeof(Enums.File));
            Enums.SquareType[] squareTypes = (Enums.SquareType[])Enum.GetValues(typeof(Enums.SquareType));
            int r;
            int f = 0;

            if (gameType.Equals("Hnefatafl"))
            {
                r = ranks.Count();
                blackPieces = 24;
                whitePieces = 12;
            }
            else if (gameType.Equals("Brandubh"))
            {
                r = ranks.Count() - 4;
                blackPieces = 8;
                whitePieces = 4;
            }
            else if (gameType.Equals("Ard Ri"))
            {
                r = ranks.Count() - 4;
                blackPieces = 16;
                whitePieces = 8;
            }
            else if (gameType.Equals("Tablut"))
            {
                r = ranks.Count() - 2;
                blackPieces = 16;
                whitePieces = 8;
            }
            else if (gameType.Equals("Tawlbwrdd"))
            {
                r = ranks.Count();
                blackPieces = 24;
                whitePieces = 12;
            }
            else
            {
                r = ranks.Count();
            }

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    // For border squares of the array, set the squares to null
                    if ((i == 0) || (j == 0) || (i == size - 1) || (j == size - 1))
                    {
                        SetSquare(null, i, j);
                    }
                    // Otherwise, create a new square at the specified location
                    else
                    {
                        SetSquare(new Square(ranks[r], files[f], null, squareTypes[2]), i, j);
                        ++f;
                    }
                }
                f = 0;
                --r;
            }

            // Set the neighbours for each square
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if ((i == 0) || (j == 0) || (i == size - 1) || (j == size - 1))
                    {
                        
                    }
                    else
                    {
                        SetNeighbours(board[i, j]);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the current <see cref="Player"/> for this <see cref="Board"/>.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to set as the <see cref="currentPlayer"/> of this <see cref="Board"/>.</param>
        public void setCurrentPlayer(Player player)
        {
            currentPlayer = player;
        }

        /// <summary>
        /// gets the current <see cref="Player"/> for this <see cref="Board"/>
        /// </summary>
        /// <returns><see cref="currentPlayer"/></returns>
        public Player getCurrentPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// Returns the <see cref="gameType"/> of this <see cref="Board"/>
        /// </summary>
        /// <returns><see cref="gameType"/></returns>
        public String getGameType()
        {
            return gameType;
        }

        /// <summary>
        /// Gets the total <see cref="Piece"/> count of a particular <see cref="Enums.Color"/> on this <see cref="Board"/>
        /// </summary>
        /// <param name="color">The <see cref="Enums.Color"/> to count for.</param>
        /// <returns>The <see cref="Piece"/> count of the passed in <see cref="Enums.Color"/> for this <see cref="Board"/>.</returns>
        public int countPieces(Enums.Color color)
        {
            int count = 0;

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    if ((i == 0) || (j == 0) || (i == size - 1) || (j == size - 1))
                    {
                       
                    }
                    else
                    {
                        if(board[i,j].getPiece() != null && board[i, j].getPiece().getColor().Equals(color))
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Returns the array containing the board data.
        /// </summary>
        /// <returns><see cref="board"/></returns>
        public Square[,] GetBoard()
        {
            return board;
        }

        /// <summary>
        /// Returns the index position in the <see cref="board"/> array corresponding to the passed in rankfile location.
        /// </summary>
        /// <param name="location">A <see cref="Tuple"/> containing the <see cref="Enums.Rank"/> and <see cref="Enums.File"/> of a square on the board.</param>
        /// <returns>A <see cref="Tuple"/> containing the index location of a position on the board.</returns>
        public Tuple<int, int> GetPosition(Tuple<Enums.Rank,Enums.File> location)
        {
            int i = 0;
            int j = 0;
            Tuple<int, int> position = new Tuple<int, int>(0, 0);
            for(i = 1; i < size-1; ++i)
            {
                for (j = 1; j < size - 1; ++j)
                {
                    if (board[i, j].getLocation().Equals(location))
                    {
                        position = new Tuple<int, int>(i, j);
                        return position;
                    }
                }
            }
            return position;
        }

        /// <summary>
        /// Returns the <see cref="Square"/> at the specified location
        /// </summary>
        /// <param name="i">The row that the <see cref="Square"/> is on.</param>
        /// <param name="j">The column that the <see cref="Square"/> is on.</param>
        /// <returns></returns>
        public Square GetSquare(int i, int j)
        {
            return board[i, j];
        }

        /// <summary>
        /// Sets the passed in location in the <see cref="board"/> array to the <see cref="Square"/> passed in.
        /// </summary>
        /// <param name="square">The <see cref="Square"/> object to set the location to.</param>
        /// <param name="i">The row of the <see cref="board"/> array.</param>
        /// <param name="j">The column of the <see cref="board"/> array.</param>
        public void SetSquare(Square square, int i, int j)
        {
            board[i, j] = square;
        }

        /// <summary>
        /// Sets and defines the neighbour locations of a <see cref="Square"/> in the <see cref="board"/> array.
        /// </summary>
        /// <param name="square">The <see cref="Square"/> to set the neighbours for.</param>
        public void SetNeighbours(Square square)
        {
            Tuple<int, int> squarePosition = GetPosition(square.getLocation());
            Square leftSquare;
            Square rightSquare;
            Square topSquare;
            Square bottomSquare;

            if(squarePosition.Item2-1 > 0)
            {
                leftSquare = GetSquare(squarePosition.Item1, squarePosition.Item2 - 1);
            }
            else
            {
                leftSquare = null;
            }

            if((squarePosition.Item2 + 1 < size - 1)){
                rightSquare = GetSquare(squarePosition.Item1, squarePosition.Item2 + 1);
            }
            else
            {
                rightSquare = null;
            }

            if ((squarePosition.Item1 - 1 > 0))
            {
                topSquare = GetSquare(squarePosition.Item1 - 1, squarePosition.Item2);
            }
            else
            {
                topSquare = null;
            }

            if ((squarePosition.Item1 + 1 < size - 1))
            {
                bottomSquare = GetSquare(squarePosition.Item1 +1, squarePosition.Item2);
            }
            else
            {
                bottomSquare = null;
            }

            square.setNeighbours(leftSquare, rightSquare, topSquare, bottomSquare);
        }

        /// <summary>
        /// Sets the special <see cref="Enums.SquareType"/>s of a <see cref="Square"/> on the <see cref="Board"/>.
        /// </summary>
        public void SetSpecialSquares()
        {
            // Set Corners
            board[1, 1].setType(Enums.SquareType.CORNER);
            board[1, size - 2].setType(Enums.SquareType.CORNER);
            board[size - 2, 1].setType(Enums.SquareType.CORNER);
            board[size - 2, size - 2].setType(Enums.SquareType.CORNER);

            // Set Throne
            board[(size / 2), (size / 2)].setType(Enums.SquareType.THRONE);
        }

        /// <summary>
        /// Returns the size of the <see cref="board"/> array.
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return size;
        }

        /// <summary>
        /// Sets the position of the <see cref="Piece"/> objects on the <see cref="Board"/> depending on the gametype
        /// </summary>
        public void SetPieces()
        {
            switch (gameType)
            {
                case "Brandubh":
                    setBrandubhPieces();
                    break;
                case "Hnefatafl":
                    setHnefataflPieces();
                    break;
                case "Ard Ri":
                    setArdRiPieces();
                    break;
                case "Tablut":
                    setTablutPieces();
                    break;
                case "Tawlbwrdd":
                    setTawlbwrddPieces();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the starting position for the Hnefatafl pieces.
        /// </summary>
        private void setHnefataflPieces()
        {
            // Top Pawns Black
            board[1, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 4].getRank(), board[1, 4].getFile()));
            board[1, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 5].getRank(), board[1, 5].getFile()));
            board[1, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 6].getRank(), board[1, 6].getFile()));
            board[1, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 7].getRank(), board[1, 7].getFile()));
            board[1, 8].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 8].getRank(), board[1, 8].getFile()));
            board[2, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 6].getRank(), board[2, 6].getFile()));

            // Left Pawns Black
            board[4, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 1].getRank(), board[4, 1].getFile()));
            board[5, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 1].getRank(), board[5, 1].getFile()));
            board[6, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 1].getRank(), board[6, 1].getFile()));
            board[7, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 1].getRank(), board[7, 1].getFile()));
            board[8, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[8, 1].getRank(), board[8, 1].getFile()));
            board[6, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 2].getRank(), board[6, 2].getFile()));

            // Bottom Pawns Black
            board[11, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 4].getRank(), board[11, 4].getFile()));
            board[11, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 5].getRank(), board[11, 5].getFile()));
            board[11, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 6].getRank(), board[11, 6].getFile()));
            board[11, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 7].getRank(), board[11, 7].getFile()));
            board[11, 8].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 8].getRank(), board[11, 8].getFile()));
            board[10, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[10, 6].getRank(), board[10, 6].getFile()));

            // Right Pawns Black
            board[4, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 11].getRank(), board[4, 11].getFile()));
            board[5, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 11].getRank(), board[5, 11].getFile()));
            board[6, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 11].getRank(), board[6, 11].getFile()));
            board[7, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 11].getRank(), board[7, 11].getFile()));
            board[8, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[8, 11].getRank(), board[8, 11].getFile()));
            board[6, 10].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 10].getRank(), board[6, 10].getFile()));

            // White Pawns
            board[4, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 6].getRank(), board[4, 6].getFile()));
            board[5, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 5].getRank(), board[5, 5].getFile()));
            board[5, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 6].getRank(), board[5, 6].getFile()));
            board[5, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 7].getRank(), board[5, 7].getFile()));
            board[6, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 4].getRank(), board[6, 4].getFile()));
            board[6, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 5].getRank(), board[6, 5].getFile()));
            board[6, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 7].getRank(), board[6, 7].getFile()));
            board[6, 8].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 8].getRank(), board[6, 8].getFile()));
            board[7, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 5].getRank(), board[7, 5].getFile()));
            board[7, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 6].getRank(), board[7, 6].getFile()));
            board[7, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 7].getRank(), board[7, 7].getFile()));
            board[8, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[8, 6].getRank(), board[8, 6].getFile()));

            // White King
            board[6, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.KING, board[6, 6].getRank(), board[6, 6].getFile()));
        }

        /// <summary>
        /// Sets the starting position for the Tawlbwrdd pieces.
        /// </summary>
        private void setTawlbwrddPieces()
        {
            // Top Pawns Black
            board[2, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 5].getRank(), board[2, 5].getFile()));
            board[1, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 5].getRank(), board[1, 5].getFile()));
            board[1, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 6].getRank(), board[1, 6].getFile()));
            board[1, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 7].getRank(), board[1, 7].getFile()));
            board[2, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 7].getRank(), board[2, 7].getFile()));
            board[3, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[3, 6].getRank(), board[3, 6].getFile()));

            // Left Pawns Black
            board[5, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 2].getRank(), board[5, 2].getFile()));
            board[5, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 1].getRank(), board[5, 1].getFile()));
            board[6, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 1].getRank(), board[6, 1].getFile()));
            board[7, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 1].getRank(), board[7, 1].getFile()));
            board[7, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 2].getRank(), board[7, 2].getFile()));
            board[6, 3].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 3].getRank(), board[6, 3].getFile()));

            // Bottom Pawns Black
            board[10, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[10, 5].getRank(), board[10, 5].getFile()));
            board[11, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 5].getRank(), board[11, 5].getFile()));
            board[11, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 6].getRank(), board[11, 6].getFile()));
            board[11, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[11, 7].getRank(), board[11, 7].getFile()));
            board[10, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[10, 7].getRank(), board[10, 7].getFile()));
            board[9, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[9, 6].getRank(), board[9, 6].getFile()));

            // Right Pawns Black
            board[5, 10].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 10].getRank(), board[5, 10].getFile()));
            board[5, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 11].getRank(), board[5, 11].getFile()));
            board[6, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 11].getRank(), board[6, 11].getFile()));
            board[7, 11].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 11].getRank(), board[7, 11].getFile()));
            board[7, 10].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 10].getRank(), board[7, 10].getFile()));
            board[6, 9].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 9].getRank(), board[6, 9].getFile()));

            // White Pawns
            board[4, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 6].getRank(), board[4, 6].getFile()));
            board[5, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 5].getRank(), board[5, 5].getFile()));
            board[5, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 6].getRank(), board[5, 6].getFile()));
            board[5, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 7].getRank(), board[5, 7].getFile()));
            board[6, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 4].getRank(), board[6, 4].getFile()));
            board[6, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 5].getRank(), board[6, 5].getFile()));
            board[6, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 7].getRank(), board[6, 7].getFile()));
            board[6, 8].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 8].getRank(), board[6, 8].getFile()));
            board[7, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 5].getRank(), board[7, 5].getFile()));
            board[7, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 6].getRank(), board[7, 6].getFile()));
            board[7, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 7].getRank(), board[7, 7].getFile()));
            board[8, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[8, 6].getRank(), board[8, 6].getFile()));

            // White King
            board[6, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.KING, board[6, 6].getRank(), board[6, 6].getFile()));
        }

        /// <summary>
        /// Sets the starting position for the Tablut pieces.
        /// </summary>
        private void setTablutPieces()
        {
            // Top Pawns Black
            board[1, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 4].getRank(), board[1, 4].getFile()));
            board[1, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 5].getRank(), board[1, 5].getFile()));
            board[1, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 6].getRank(), board[1, 6].getFile()));
            board[2, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 5].getRank(), board[2, 5].getFile()));

            // Left Pawns Black
            board[4, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 1].getRank(), board[4, 1].getFile()));
            board[5, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 1].getRank(), board[5, 1].getFile()));
            board[6, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 1].getRank(), board[6, 1].getFile()));
            board[5, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 2].getRank(), board[5, 2].getFile()));

            // Bottom Pawns Black
            board[8, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[8, 5].getRank(), board[8, 5].getFile()));
            board[9, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[9, 4].getRank(), board[9, 4].getFile()));
            board[9, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[9, 5].getRank(), board[9, 5].getFile()));
            board[9, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[9, 6].getRank(), board[9, 6].getFile()));

            // Right Pawns Black
            board[5, 8].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 8].getRank(), board[5, 8].getFile()));
            board[4, 9].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 9].getRank(), board[4, 9].getFile()));
            board[5, 9].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 9].getRank(), board[5, 9].getFile()));
            board[6, 9].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 9].getRank(), board[6, 9].getFile()));

            // White Pawns
            board[3, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[3, 5].getRank(), board[3, 5].getFile()));
            board[4, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 5].getRank(), board[4, 5].getFile()));
            board[5, 3].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 3].getRank(), board[5, 3].getFile()));
            board[5, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 4].getRank(), board[5, 4].getFile()));
            board[5, 6].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 6].getRank(), board[5, 6].getFile()));
            board[5, 7].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 7].getRank(), board[5, 7].getFile()));
            board[6, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[6, 5].getRank(), board[6, 5].getFile()));
            board[7, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[7, 5].getRank(), board[7, 5].getFile()));

            // White King
            board[5, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.KING, board[5, 5].getRank(), board[5, 5].getFile()));
        }

        /// <summary>
        /// Sets the starting position for the Brandubh pieces.
        /// </summary>
        private void setBrandubhPieces()
        {
            // Top Pawns Black
            board[1, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 4].getRank(), board[1, 4].getFile()));
            board[2, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 4].getRank(), board[2, 4].getFile()));

            // Left Pawns Black
            board[4, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 1].getRank(), board[4, 1].getFile()));
            board[4, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 2].getRank(), board[4, 2].getFile()));

            // Right Pawns Black
            board[4, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 7].getRank(), board[4, 7].getFile()));
            board[4, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 6].getRank(), board[4, 6].getFile()));

            // Bottom Pawns Black
            board[7, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 4].getRank(), board[7, 4].getFile()));
            board[6, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 4].getRank(), board[6, 4].getFile()));

            // White Pawns
            board[4, 3].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 3].getRank(), board[4, 3].getFile()));
            board[4, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 5].getRank(), board[4, 5].getFile()));
            board[3, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[3, 4].getRank(), board[3, 4].getFile()));
            board[5, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 4].getRank(), board[5, 4].getFile()));

            // White King
            board[4, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.KING, board[4, 4].getRank(), board[4, 4].getFile()));
        }

        /// <summary>
        /// Sets the starting position for the Ard Rí pieces.
        /// </summary>
        private void setArdRiPieces()
        {
            // Top Pawns Black
            board[1, 3].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 3].getRank(), board[1, 3].getFile()));
            board[1, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 4].getRank(), board[1, 4].getFile()));
            board[1, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[1, 5].getRank(), board[1, 5].getFile()));
            board[2, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[2, 4].getRank(), board[2, 4].getFile()));

            // Left Pawns Black
            board[3, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[3, 1].getRank(), board[3, 1].getFile()));
            board[4, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 1].getRank(), board[4, 1].getFile()));
            board[5, 1].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 1].getRank(), board[5, 1].getFile()));
            board[4, 2].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 2].getRank(), board[4, 2].getFile()));

            // Right Pawns Black
            board[3, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[3, 7].getRank(), board[3, 7].getFile()));
            board[4, 6].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 6].getRank(), board[4, 6].getFile()));
            board[5, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[5, 7].getRank(), board[5, 7].getFile()));
            board[4, 7].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[4, 7].getRank(), board[4, 7].getFile()));

            // Bottom Pawns Black
            board[7, 3].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 3].getRank(), board[7, 3].getFile()));
            board[7, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 4].getRank(), board[7, 4].getFile()));
            board[7, 5].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[7, 5].getRank(), board[7, 5].getFile()));
            board[6, 4].setPiece(new Piece(Enums.Color.BLACK, Enums.PieceType.PAWN, board[6, 4].getRank(), board[6, 4].getFile()));

            // White Pawns
            board[4, 3].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 3].getRank(), board[4, 3].getFile()));
            board[4, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[4, 5].getRank(), board[4, 5].getFile()));
            board[3, 3].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[3, 3].getRank(), board[3, 3].getFile()));
            board[3, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[3, 4].getRank(), board[3, 4].getFile()));
            board[3, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[3, 5].getRank(), board[3, 5].getFile()));
            board[5, 3].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 3].getRank(), board[5, 3].getFile()));
            board[5, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 4].getRank(), board[5, 4].getFile()));
            board[5, 5].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.PAWN, board[5, 5].getRank(), board[5, 5].getFile()));

            // White King
            board[4, 4].setPiece(new Piece(Enums.Color.WHITE, Enums.PieceType.KING, board[4, 4].getRank(), board[4, 4].getFile()));
        }

        /// <summary>
        /// Prints a representation of the <see cref="board"/>
        /// </summary>
        /// <returns>A <see cref="string"/> <see cref="Array"/> representing the <see cref="board"/> </returns>
        public string[] PrintBoard()
        {
            string[] boardString = new string[size];
            int k = 0;
            for(int i = 0; i < size; i++)
            {
                boardString[k] = "";
                for(int j = 0; j < size; j++)
                {

                    if(board[i,j] == null)
                    {
                        boardString[k] += "|---";
                    }
                    else
                    {
                        if (board[i, j].getSquareType() == Enums.SquareType.CORNER)
                        {
                            boardString[k] += "|-X-";
                        }
                        else if(board[i,j].getSquareType() == Enums.SquareType.REGULAR)
                        {
                            if(board[i, j].getPiece() != null)
                            {
                                if(board[i, j].getPiece().getColor().Equals(Enums.Color.BLACK))
                                {
                                    boardString[k] += "|-B-";
                                }
                                else if (board[i, j].getPiece().getColor().Equals(Enums.Color.WHITE))
                                {
                                    boardString[k] += "|-W-";
                                }
                                
                            }
                            else
                            {
                                boardString[k] += "|-0-";
                            }
                        }
                        else if(board[i,j].getSquareType() == Enums.SquareType.THRONE)
                        {
                            if (board[i, j].getPiece() != null)
                            {
                                boardString[k] += "|-K-";
                            }
                            else
                            {
                                boardString[k] += "|-T-";
                            }
                        }
                    }

                }
                boardString[k] += "|";
                boardString[k] += "\n";
                k++;
            }
            return boardString;
        }

        /// <summary>
        /// A <see cref="String"/> representation of this <see cref="Board"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> representation of this <see cref="Board"/>.</returns>
        public override String ToString()
        {
            String theString = "";
            int size = GetSize();

            theString += "\n | Size : " + size;
            theString += "\n | Current Player  : " + getCurrentPlayer().getColor();
            theString += "\n | Game Type  : " + getGameType();
            theString += "\n | Black Pieces : " + countPieces(Enums.Color.BLACK);
            theString += "\n | White Pieces : " + countPieces(Enums.Color.WHITE);

            return theString;
        }
    }
}
