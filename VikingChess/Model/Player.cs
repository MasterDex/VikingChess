using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikingChess.Model
{
    /// <summary>
    /// The <see cref="Player"/> class represents a player in the <see cref="Game"/>
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The <see cref="Enums.Color"/> of the <see cref="Player"/>
        /// </summary>
        private Enums.Color color;
        /// <summary>
        /// A <see cref="List{T}"/> containing the moves made by the player over the course of a <see cref="Game"/>
        /// </summary>
        private List<String> moveList;
        /// <summary>
        /// The name of the <see cref="Player"/>
        /// </summary>
        private string name = "";
        /// <summary>
        /// The <see cref="Enums.PlayerType"/> of the <see cref="Player"/>
        /// </summary>
        private Enums.PlayerType type;
        /// <summary>
        /// A <see cref="bool"/> representing whether or not the <see cref="Player"/> has won or not.
        /// </summary>
        private Boolean hasWon = false;

        #region Constructors

        /// <summary>
        /// Simple Constructor
        /// </summary>
        public Player()
        {
            moveList = new List<String>();
        }

        /// <summary>
        /// Constructs a new <see cref="Player"/> object and sets its color.
        /// </summary>
        /// <param name="color"><see cref="color"/></param>
        public Player(Enums.Color color, Enums.PlayerType type)
        {
            setColor(color);
            setType(type);
            moveList = new List<String>();
        }

        /// <summary>
        /// Main Constructor
        /// </summary>
        /// <param name="color"><see cref="color"/></param>
        /// <param name="name"><see cref="name"/></param>
        /// <param name="type"><see cref="type"/></param>
        public Player(Enums.Color color, string name, Enums.PlayerType type)
        {
            setColor(color);
            setType(type);
            setName(name);
            moveList = new List<String>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to copy</param>
        public Player(Player player)
        {
            setColor(player.getColor());
            setType(player.getType());
            setName(player.getName());
            moveList = player.getMoveList();
            hasWon = player.hasWon;
        }

        #endregion

        #region Getters & Setters

        /// <summary>
        /// Returns the <see cref="Enums.Color"/> of the <see cref="Player"/>.
        /// </summary>
        /// <returns><see cref="color"/></returns>
        public Enums.Color getColor()
        {
            return color;
        }

        /// <summary>
        /// Returns the <see cref="name"/> of the <see cref="Player"/>.
        /// </summary>
        /// <returns><see cref="name"/></returns>
        public string getName()
        {
            return name;
        }

        /// <summary>
        /// Returns the <see cref="Enums.PlayerType"/> of the <see cref="Player"/>
        /// </summary>
        /// <returns><see cref="type"/></returns>
        public Enums.PlayerType getType()
        {
            return type;
        }

        /// <summary>
        /// Returns the win status of the <see cref="Player"/>
        /// </summary>
        /// <returns><see cref="hasWon"/></returns>
        public Boolean getWin()
        {
            return hasWon;
        }

        /// <summary>
        /// Returns the <see cref="moveList"/> of the <see cref="Player"/>.
        /// </summary>
        /// <returns><see cref="moveList"/></returns>
        public List<String> getMoveList()
        {
            return moveList;
        }

        /// <summary>
        /// Sets the <see cref="Enums.Color"/> of the <see cref="Player"/>.
        /// </summary>
        /// <param name="color"><see cref="color"/></param>
        public void setColor(Enums.Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Sets the <see cref="name"/> of the <see cref="Player"/>
        /// </summary>
        /// <param name="name"><see cref="name"/></param>
        public void setName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Sets the <see cref="Enums.PlayerType"/> of <see cref="Player"/>
        /// </summary>
        /// <param name="type"><see cref="type"/></param>
        public void setType(Enums.PlayerType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Sets the win status of the <see cref="Player"/>.
        /// </summary>
        /// <param name="win"><see cref="hasWon"/></param>
        public void setWin(bool win)
        {
            hasWon = win;
        }

        #endregion

        #region Miscellaneous methods

        /// <summary>
        /// Adds a new entry to the <see cref="moveList"/> of the <see cref="Player"/>
        /// </summary>
        /// <param name="moveNotation">A string containing the move notation</param>
        public void updateMoveList(string moveNotation)
        {
            moveList.Add(moveNotation);
        }

        /// <summary>
        /// Removes an entry from the <see cref="moveList"/> of the <see cref="Player"/>.
        /// </summary>
        public void removeFromMoveList()
        {
            moveList.RemoveAt(moveList.Count - 1);
        }

        /// <summary>
        /// Use the MCTS algorithm to make a move
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use for the move</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing the origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/> to move.</returns>
        public Tuple<Square, Piece, Square> makeMCTSMove(Board board)
        {
            MCTS mctsTree = new MCTS(board);

            Node node = mctsTree.makeMove(board);
            Tuple<Square, Piece, Square> moveTuple = node.GetLastMove();
            return moveTuple;
        }


        /// <summary>
        /// Returns a <see cref="List{T}"/> containing all <see cref="Square"/> objects that have <see cref="Piece"/> objects that
        /// can be moved.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to get all <see cref="Square"/> objects from.</param>
        /// <returns><see cref="List{T}"/> of <see cref="Square"/> objects containing <see cref="Piece"/> objects.</returns>
        public List<Square> GetAllPieceSquares(Board board)
        {
            List<Square> squareList = new List<Square>();

            // Loop through the board and get all squares with moveable pieces
            for (int i = 0; i < board.GetSize(); ++i)
            {
                for (int j = 0; j < board.GetSize(); ++j)
                {
                    Square currSq = board.GetSquare(i, j);

                    if (currSq != null && currSq.getPiece() != null)
                    {
                        Piece currPiece = currSq.getPiece();

                        if (currPiece.getColor().Equals(this.getColor()) && currPiece.generateLegalMoves(board).Count != 0)
                        {
                            squareList.Add(currSq);
                        }
                    }
                }
            }
            return squareList;
        }

        /// <summary>
        /// Select the King <see cref="Piece"/> from the <see cref="List{T}"/> of moveable pieces
        /// </summary>
        /// <param name="squareList">A <see cref="List{T}"/> cntaining all <see cref="Square"/> objects that have <see cref="Piece"/> objects.</param>
        /// <param name="board">The <see cref="Board"/> to use.</param>
        /// <returns>The <see cref="Square"/> containing the King <see cref="Piece"/></returns>
        private Square SelectKingSquare(List<Square> squareList, Board board)
        {
            //Loop through all moveable pieces
            for (int i = 0; i < squareList.Count; ++i)
            {
                Square currSq = squareList[i] as Square;
                Piece currPiece = currSq.getPiece();

                if (currPiece.getType().Equals(Enums.PieceType.KING))
                {
                    List<Square> kingMoves = currPiece.generateLegalMoves(board);

                    for (int j = 0; j < kingMoves.Count; ++j)
                    {
                        Square moveSq = kingMoves[j] as Square;
                        Tuple<Enums.Rank,Enums.File> sqLoc = moveSq.getLocation();

                        for(int k = 1; k < board.GetSize()-2; ++k)
                        {
                            Square[,] boardArr = board.GetBoard();

                            // If the king can move to an edge square
                            if(boardArr[1,k].getLocation().Equals(sqLoc) || boardArr[k,1].getLocation().Equals(sqLoc) || 
                                boardArr[board.GetSize()-2, k].getLocation().Equals(sqLoc) || boardArr[k, board.GetSize() - 2].getLocation().Equals(sqLoc))
                            {
                                return currSq;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Select a <see cref="Piece"/> that can capture another <see cref="Piece"/>.
        /// </summary>
        /// <param name="squareList"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        private Square SelectCaptureSquare(List<Square> squareList, Board board)
        {
            // Select a piece that can capture another piece
            for (int i = 0; i < squareList.Count; ++i)
            {
                Square currSq = squareList[i] as Square;
                List<Square> currMoves = currSq.getPiece().generateLegalMoves(board);

                // Get the neighbours of the current square
                for (int j = 0; j < currMoves.Count; ++j)
                {
                    Square moveSquare = currMoves[j] as Square;
                    // Get the location of this square
                    Tuple<Enums.Rank, Enums.File> sqLoc = moveSquare.getLocation();

                    //Get the neighbours of the current move
                    Square[] neighbours = moveSquare.getNeighbours();

                    //Check the neighbouring squares for a piece of the opposite color
                    for (int k = 0; k < neighbours.Length; ++k)
                    {
                        Square neighbour = neighbours[k] as Square;
                        if (neighbour != null && neighbour.getPiece() != null)
                        {
                            //If the piece on the neighbouring square is different to the player's color
                            if (!(neighbour.getPiece().getColor().Equals(getColor())))
                            {
                                // Get the neighbours of this square
                                Square[] neighboursNeighbours = neighbour.getNeighbours();

                                for (int l = 0; l < neighboursNeighbours.Length; ++l)
                                {
                                    Square neighboursNeighbour = neighboursNeighbours[l] as Square;
                                    if (neighboursNeighbour != null && neighboursNeighbour.getPiece() != null)
                                    {
                                        // If the location of the nieghboursNeighbour is different to the location of the current move square but has the same rank or file as the moveSquare
                                        if (!(neighboursNeighbour.getLocation().Equals(sqLoc)) && (neighboursNeighbour.getRank().Equals(moveSquare.getRank()) || neighboursNeighbour.getFile().Equals(moveSquare.getFile())))
                                            if (neighboursNeighbour.getPiece().getColor().Equals(getColor()))
                                                return currSq;

                                    }
                                    else if (getColor().Equals(Enums.Color.BLACK) && neighbour.getPiece().Equals(Enums.PieceType.KING))
                                    {
                                        return currSq;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Selects a <see cref="Square"/> containing a valid <see cref="Piece"/> for the CPU <see cref="Player"/>
        /// </summary>
        /// <param name="board">The current state of the <see cref="Board"/></param>
        /// <returns>A <see cref="Square"/> containing a valid <see cref="Piece"/> for the CPU</returns>
        public Square selectPieceSquare(Board board)
        {
            List<Square> squareList = new List<Square>();
            Square pieceSquare = null;

            // Get moveable pieces
            squareList = GetAllPieceSquares(board);

            if(squareList.Count == 0)
            {
                Debug.WriteLine("Error: squareList.Count == 0");
                return null;
            }
            if(pieceSquare == null)
            {
                pieceSquare = SelectKingSquare(squareList, board);
            }
            if(pieceSquare == null)
            {
                // Otherwise return a random square
                Random rnd = new Random();
                int index = rnd.Next(0, squareList.Count);
                pieceSquare = squareList[index] as Square;
            }
            return pieceSquare;
        }

        /// <summary>
        /// Returns a <see cref="Tuple{T1, T2, T3}"/> containing the origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/> necessary to move.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use when obtaining the move tuple</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing the origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/> necessary to move.</returns>
        public Tuple<Square,Piece,Square> getMoveTuple(Board board)
        {
            Square originSquare = selectPieceSquare(board);
            Piece selectedPiece = originSquare.getPiece();
            Square destinationSquare;

            List<Square> moveList = selectedPiece.generateLegalMoves(board);

            if (moveList.Count == 0)
            {
                Debug.WriteLine("Error: squareList.Count == 0");
                return null;
            }

            Tuple<Square, Piece, Square> moveTuple;
        
            // Otherwise, pick a random destination
            Random rnd = new Random();
            int index = rnd.Next(0, moveList.Count);
            destinationSquare = moveList[index] as Square;
            
            moveTuple = new Tuple<Square, Piece, Square>(originSquare,selectedPiece,destinationSquare);

            return moveTuple;
        }

        /// <summary>
        /// Tries to return a <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/>
        /// that will capture another <see cref="Piece"/>
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use.</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/></returns>
        public Tuple<Square, Piece, Square> capturePiece(Board board)
        {
            Square originSquare = selectPieceSquare(board);
            Piece selectedPiece = originSquare.getPiece();
            Square destinationSquare;

            List<Square> moveList = selectedPiece.generateLegalMoves(board);
            Tuple<Square, Piece, Square> moveTuple;

            // If the selected piece can capture another piece, move to capture
            foreach (Square m in moveList.ToList())
            {
                Square moveSquare = new Square(m);
                // Get the location of this square
                Tuple<Enums.Rank, Enums.File> sqLoc = moveSquare.getLocation();

                //Get the neighbours of the current move
                Square[] neighbours = moveSquare.getNeighbours();

                //Check the neighbouring squares for a piece of the opposite color
                for (int j = 0; j < neighbours.Length; ++j)
                {
                    Square neighbour = neighbours[j] as Square;
                    if (neighbour != null && neighbour.getPiece() != null)
                    {
                        //If the piece on the neighbouring square is different to the player's color
                        if ( !neighbour.getPiece().getColor().Equals(getColor()) )
                        {
                            // Get the neighbours of this square
                            Square[] neighboursNeighbours = neighbour.getNeighbours();

                            for (int k = 0; k < neighboursNeighbours.Length; ++k)
                            {
                                Square neighboursNeighbour = neighboursNeighbours[k] as Square;
                                if (neighboursNeighbour != null && neighboursNeighbour.getPiece() != null)
                                {
                                    // If the location of the nieghboursNeighbour is different to the location of the current move square
                                    if ((!neighboursNeighbour.getLocation().Equals(sqLoc)) && ( neighboursNeighbour.getRank().Equals(moveSquare.getRank()) || neighboursNeighbour.getFile().Equals(moveSquare.getFile()) ) )
                                    {
                                        if (neighboursNeighbour.getPiece().getColor().Equals(getColor()))
                                        {
                                            destinationSquare = moveSquare;
                                            moveTuple = new Tuple<Square, Piece, Square>(originSquare, selectedPiece, destinationSquare);

                                            return moveTuple;
                                        }
                                    }
                                }
                               else if (getColor().Equals(Enums.Color.BLACK) && neighbour.getPiece().Equals(Enums.PieceType.KING))
                                {
                                    destinationSquare = moveSquare;
                                    moveTuple = new Tuple<Square, Piece, Square>(originSquare, selectedPiece, destinationSquare);

                                    return moveTuple;
                                }

                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to return a <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/>
        /// that will capture the King <see cref="Piece"/>
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use.</param>
        /// <param name="squareList">A <see cref="List{T}"/> containing all <see cref="Square"/> objects that have <see cref="Piece"/> objects on them.</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/></returns>
        public Tuple<Square, Piece, Square> captureKing(Board board, List<Square> squareList)
        {
            // Check if the King can be captured
            for (int i = 1; i < board.GetSize()-2; ++i)
            {
                for (int j = 1; j < board.GetSize()-2; ++j)
                {
                    Square currSquare = board.GetSquare(i, j);

                    if (currSquare != null)
                    {
                        Piece currPiece = currSquare.getPiece();
                        if (currPiece != null && currPiece.getType().Equals(Enums.PieceType.KING))
                        {
                            Square kingSquare = currSquare;
                            Tuple<int, int> kingIndex = board.GetPosition(new Tuple<Enums.Rank, Enums.File>(kingSquare.getRank(), kingSquare.getFile()));
                            Square[] kingNeighbours = kingSquare.getNeighbours();
                            List<Square> kingMoves = kingSquare.getPiece().generateLegalMoves(board);

                            for (int k = 0; k < squareList.Count; ++k)
                            {
                                Square selectedSquare = squareList[k] as Square;
                                Piece selectedPiece = selectedSquare.getPiece();
                                List<Square> moves = selectedPiece.generateLegalMoves(board);
                                if (selectedPiece.getColor().Equals(Enums.Color.BLACK))
                                {
                                    Square blockingSquare = null;

                                    // First see if the King can escape and find the square to block it.
                                    for (int l = 0; l < kingMoves.Count; ++l)
                                    {
                                        for (int m = 1; m < board.GetSize() - 2; ++m)
                                        {
                                            Square currMove = kingMoves[l] as Square;
                                            // Top
                                            if (board.GetSquare(1, m).Equals(currMove))
                                            {
                                                blockingSquare = board.GetSquare(kingIndex.Item1 - 1, m);
                                            }
                                            // Left
                                            else if (board.GetSquare(m, 1).Equals(currSquare))
                                            {
                                                blockingSquare = board.GetSquare(m, kingIndex.Item2 - 1);
                                            }
                                            // Bottom
                                            else if (board.GetSquare(board.GetSize() - 2, m).Equals(currSquare))
                                            {
                                                blockingSquare = board.GetSquare(kingIndex.Item1 + 1, m);
                                            }
                                            // Right
                                            else if (board.GetSquare(m, board.GetSize() - 2).Equals(currSquare))
                                            {
                                                blockingSquare = board.GetSquare(m, kingIndex.Item2 + 1);
                                            }
                                        }

                                    }

                                    Square destination = null;

                                    for (int l = 0; l < moves.Count; ++l)
                                    {
                                        // If you can block the king then do so
                                        if (blockingSquare != null)
                                        {
                                            if (moves[l].Equals(blockingSquare))
                                            {
                                                Tuple<Square, Piece, Square> moveTuple = new Tuple<Square, Piece, Square>(selectedSquare, selectedPiece, blockingSquare);

                                                return moveTuple;
                                            }
                                        }
                                        else
                                        {
                                            for (int m = 0; m < kingNeighbours.Length; ++m)
                                            {
                                                if (moves[l].Equals(kingNeighbours[m]))
                                                {
                                                    destination = moves[l] as Square;

                                                    Tuple<Square, Piece, Square> moveTuple = new Tuple<Square, Piece, Square>(selectedSquare, selectedPiece, destination);

                                                    return moveTuple;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Tries to return a <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/>
        /// that will allow the King to escape
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use.</param>
        /// <param name="squareList">A <see cref="List{T}"/> containing all <see cref="Square"/> objects that have <see cref="Piece"/> objects on them.</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/></returns>
        public Tuple<Square, Piece, Square> escapeKing(Board board, List<Square> squareList)
        {
            Square destination = null;

            for(int i = 0; i < squareList.Count; ++ i)
            {
                Square selectedSquare = squareList[i] as Square;

                if(selectedSquare != null && selectedSquare.getPiece().getType().Equals(Enums.PieceType.KING))
                {
                    Square kingSquare = selectedSquare;
                    Piece kingPiece = selectedSquare.getPiece();

                    List<Square> moveList = kingPiece.generateLegalMoves(board);

                    for(int j = 0; j < moveList.Count; ++j)
                    {
                        for(int k = 1; k < board.GetSize()-2; ++k)
                        {
                            Square currSquare = moveList[j] as Square;
                            // Top
                            if (board.GetSquare(1, k).Equals(currSquare))
                            {
                                destination = currSquare;
                            }
                            // Left
                            else if(board.GetSquare(k, 1).Equals(currSquare))
                            {
                                destination = currSquare;
                            }
                            // Bottom
                            else if (board.GetSquare(board.GetSize()-2, k).Equals(currSquare))
                            {
                                destination = currSquare;
                            }
                            // Right
                            else if (board.GetSquare(k, board.GetSize()-2).Equals(currSquare))
                            {
                                destination = currSquare;
                            }
                            if(destination != null)
                            {
                                Tuple<Square, Piece, Square> moveTuple = new Tuple<Square, Piece, Square>(kingSquare, kingPiece, destination);

                                return moveTuple;
                            }
                        }
                        
                    }
                } 
            }
            return null;
        }

        /// <summary>
        /// Tries to return a <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/>
        /// that will result in a win.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use</param>
        /// <returns>A <see cref="Tuple{T1, T2, T3}"/> containing an origin <see cref="Square"/>, destination <see cref="Square"/> and <see cref="Piece"/></returns>
        public Tuple<Square, Piece, Square> GetWinningMove(Board board)
        {
            List<Square> squareList = GetAllPieceSquares(board);
            Tuple<Square, Piece, Square> moveTuple = null;

            if (getColor().Equals(Enums.Color.BLACK))
            {
                moveTuple = captureKing(board, squareList);
            }
            else if (getColor().Equals(Enums.Color.WHITE))
            {
                moveTuple = escapeKing(board, squareList);
            }
            return moveTuple;
        }

        /// <summary>
        /// <see cref="ToString"/> override
        /// </summary>
        /// <returns>A <see cref="String"/> representation of the <see cref="Player"/></returns>
        public override string ToString()
        {
            string theString = "";

            theString += "\n | Color : " + getColor().ToString();
            theString += "\n | Type  : " + getType().ToString();
            theString += "\n | Name  : " + getName();
            theString += "\n | Moves : " + getMoveList().ToString();

            return theString;
        }

        #endregion
    }
}
