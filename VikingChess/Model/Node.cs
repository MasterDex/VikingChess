using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VikingChess.Model
{
    /// <summary>
    /// The <see cref="Node"/> class defines a node within an <see cref="MCTS"/> tree.
    /// </summary>
    class Node
    {
        /// <summary>
        /// The number of times this <see cref="Node"/> has been visited.
        /// </summary>
        private int visitCount = 0;
        /// <summary>
        /// The score of this <see cref="Node"/>
        /// </summary>
        private double score = 0;
        /// <summary>
        /// The <see cref="Board"/> object representing the game board and position of pieces.
        /// </summary>
        private Board board;
        /// <summary>
        /// The parent <see cref="Node"/> of this <see cref="Node"/>
        /// </summary>
        private Node parent;
        /// <summary>
        /// An <see cref="List"/> containing any and all child <see cref="States"/> of this <see cref="Node"/>
        /// </summary>
        private List<Node> children;
        /// <summary>
        /// Defines whether or not the board contained within this node is a terminal state or not.
        /// </summary>
        private bool hasWon = false;
        /// <summary>
        /// A constant to protect against divide by zero errors in the UCT algorithm.
        /// </summary>
        private const double EPSILON = 10e-6;
        /// <summary>
        /// The last move made that resulted in the <see cref="board"/> of this node.
        /// </summary>
        private Tuple<Square, Piece, Square> lastMoveTuple;

        /// <summary>
        /// Constructs a new <see cref="Node"/>. This constructor is used to form the root of the <see cref="Node"/> tree.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> object to store.</param>
        public Node(Board board)
        {
            parent = null;
            this.board = board;

            children = new List<Node>();
        }

        /// <summary>
        /// Constructs a new <see cref="Node"/>. This constructor is used to create a child <see cref="Node"/>.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> object to store.</param>
        /// <param name="parent">The parent <see cref="Node"/> of this <see cref="Node"/></param>
        public Node(Board board, Node parent)
        {
            this.parent = parent;
            this.board = board;

            children = new List<Node>();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to copy</param>
        public Node(Node node)
        {
            parent = node.parent;
            board = node.board;
            lastMoveTuple = node.lastMoveTuple;

            children = new List<Node>(node.children);
        }

        /// <summary>
        /// Returns the <see cref="Board"/> object contained in this <see cref="Node"/>.
        /// </summary>
        /// <returns>The <see cref="Board"/> contained in this <see cref="Node"/></returns>
        public Board GetBoard()
        {
            return board;
        }

        /// <summary>
        /// Returns the <see cref="parent"/> of this <see cref="Node"/>.
        /// </summary>
        /// <returns>The <see cref="parent"/> of this <see cref="Node"/></returns>
        public Node GetParent()
        {
            return parent;
        }

        /// <summary>
        /// Returns the <see cref="children"/> of this <see cref="Node"/>.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> containing the <see cref="children"/> of this <see cref="Node"/>/></returns>
        public List<Node> GetChildren()
        {
            return children;
        }

        /// <summary>
        ///  Returns the number of times this <see cref="Node"/> has been visited.
        /// </summary>
        /// <returns><see cref="visitCount"/></returns>
        public int GetVisits()
        {
            return visitCount;
        }

        /// <summary>
        /// Returns the win status of this <see cref="Node"/>
        /// </summary>
        /// <returns><see cref="hasWon"/></returns>
        public bool getWin()
        {
            return hasWon;
        }

        /// <summary>
        /// Sets the win status of this <see cref="Node"/>
        /// </summary>
        /// <param name="winStatus">The <see cref="bool"/> win status to set.</param>
        public void setWin(bool winStatus)
        {
            hasWon = winStatus;
        }

        /// <summary>
        /// Returns the <see cref="score"/> of this <see cref="Node"/>.
        /// </summary>
        /// <returns><see cref="score"/></returns>
        public double GetScore()
        {
            return score;
        }

        /// <summary>
        /// Returns the chance that this <see cref="Node"/> will lead to a win.
        /// </summary>
        /// <returns>The chance that this node will lead to a win. This is defined as <see cref="score"/> / <see cref="visitCount"/></returns>
        public double getWinChance()
        {
            if(visitCount == 0)
            {
                return 0;
            }
            else
            {
                return score / visitCount;
            }
        }

        /// <summary>
        /// Finds the best child <see cref="Node"/> of this <see cref="Node"/>
        /// </summary>
        /// <returns>The best child <see cref="Node"/> of this <see cref="Node"/></returns>
        public Node getBestChild()
        {
            Node bestChild = children[0];
            for (int i = 1; i < children.Count; ++i)
            {
                Node currChild = children[i];
                if (bestChild.getWinChance() < currChild.getWinChance())
                {
                    bestChild = currChild;
                }
            }
            return bestChild;
        }

        /// <summary>
        /// Returns the <see cref="lastMoveTuple"/> of this <see cref="Node"/>
        /// </summary>
        /// <returns><see cref="lastMoveTuple"/></returns>
        public Tuple<Square, Piece, Square> GetLastMove()
        {
            return lastMoveTuple;
        }

        /// <summary>
        /// Sets the <see cref="lastMoveTuple"/> of this <see cref="Node"/>.
        /// </summary>
        /// <param name="moveTuple">The move that results in the <see cref="Board"/> contained in this <see cref="Node"/></param>
        public void setLastMove(Tuple<Square, Piece, Square> moveTuple)
        {
            lastMoveTuple = moveTuple;
        }

        /// <summary>
        /// Checks to see if this is a leaf <see cref="Node"/>.
        /// </summary>
        /// <returns>A <see cref="bool"/> representing whether this <see cref="Node"/> is a leaf node or not.</returns>
        public bool isLeaf()
        {
            if(children.Count <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the <see cref="visitCount"/> of this <see cref="Node"/>
        /// </summary>
        public void updateVisitCount()
        {
            ++visitCount;
        }

        /// <summary>
        /// Updates the <see cref="score"/> of this <see cref="Node"/>.
        /// </summary>
        /// <param name="value"></param>
        public void updateScore(double value)
        {
            score += value;
        }

        /// <summary>
        /// A method that updates the <see cref="score"/> and <see cref="visitCount"/> of this <see cref="Node"/>
        /// </summary>
        /// <param name="score"></param>
        public void updateStats(double score)
        {
            updateVisitCount();
            updateScore(score);
        }

        /// <summary>
        /// Adds a new child <see cref="Node"/> to this <see cref="Node"/>.
        /// </summary>
        /// <param name="board"></param>
        public void AddChild(Board board, Tuple<Square, Piece, Square> moveTuple)
        {
            Node state = new Node(board, this);
            state.setLastMove(moveTuple);
            children.Add(state);
        }

        /// <summary>
        /// Uses the UCT formula to select the most promising child <see cref="Node"/> to explore.
        /// <code>// UCT Formula
        /// uctVal = (child.GetScore() / (child.GetVisits() + EPSILON)) + Math.Sqrt(2) * Math.Sqrt((Math.Log(GetVisits() + 1)) / (child.GetVisits() + EPSILON)) + x + EPSILON;</code>
        /// </summary>
        /// <returns>The most promising child <see cref="Node"/></returns>
        public Node UCTSelectBest()
        {
            if (children.Count == 0)
            {
                return null;
            }

            Node selected = null;
            double bestValue = Double.NegativeInfinity;
            
            // Calculate the UCT value for each child node
            foreach (Node child in children)
            {
                // A small random number to aid in offsetting ties between nodes
                Random r = new Random();
                int x = r.Next(0, 2);
                double uctVal;

                // UCT Formula
                uctVal = (child.GetScore() / (child.GetVisits() + EPSILON)) +
                Math.Sqrt(2) * Math.Sqrt((Math.Log(GetVisits() + 1)) / (child.GetVisits() + EPSILON)) + x + EPSILON;

                if (uctVal > bestValue)
                {
                    selected = child;
                    bestValue = uctVal;
                }
            }
            return selected;
        }

        /// <summary>
        /// Expands this <see cref="Node"/> by creating new child <see cref="Node"/>
        /// </summary>
        public void ExpandNode()
        {
            Game game = new Game(board, true);
            Board tempBoard = new Board(board);
            Player betaPlayer = board.getCurrentPlayer();
            Tuple<Square, Piece, Square> moveTuple = null;

            try
            {
                // Make winning move for opponent if possible
                moveTuple = betaPlayer.GetWinningMove(board);
                if (moveTuple != null)
                {
                    tempBoard = new Board(board);
                    tempBoard.setCurrentPlayer(betaPlayer);
                    Game tempGame = new Game(tempBoard, true);
                    tempGame.movePiece(moveTuple.Item3, moveTuple.Item1, moveTuple.Item2);
                    if (tempGame.getCurrentPlayer().getColor().Equals(betaPlayer.getColor()))
                    {
                        tempGame.updateCurrentPlayer();
                    }
                    tempBoard.setCurrentPlayer(tempGame.getCurrentPlayer());
                    AddChild(tempBoard, moveTuple);
                    tempBoard = null;
                    tempGame = null;
                    return;
                }
                moveTuple = betaPlayer.capturePiece(board);
                if (moveTuple != null)
                {
                    // Make alternative move
                    tempBoard = new Board(board);
                    tempBoard.setCurrentPlayer(betaPlayer);
                    Game tempGame = new Game(tempBoard, true);
                    tempGame.movePiece(moveTuple.Item3, moveTuple.Item1, moveTuple.Item2);
                    if (tempGame.getCurrentPlayer().getColor().Equals(betaPlayer.getColor()))
                    {
                        tempGame.updateCurrentPlayer();
                    }
                    tempBoard.setCurrentPlayer(tempGame.getCurrentPlayer());
                    AddChild(tempBoard, moveTuple);
                    tempBoard = null;
                    tempGame = null;
                    return;
                }
                else
                {
                    // Get all pieces that can be moved by the current player
                    List<Square> squareList = betaPlayer.GetAllPieceSquares(tempBoard);
                    // Loop through the list of squares
                    foreach (Square sq in squareList.ToArray())
                    {
                        // Get a square with a moveable piece on it
                        Square origin = new Square(sq);
                        Piece selectedPiece = new Piece(sq.getPiece());

                        // Generate the possible moves for that piece
                        List<Square> moveList = sq.getPiece().generateLegalMoves(tempBoard);

                        // Loop through the move list
                        foreach (Square m in moveList.ToArray())
                        {
                            // Set the destination square to the current move
                            Square destination = new Square(m);

                            // Create a new board to hold the move
                            Board newBoard = new Board(board);
                            // Set the current player of the game to the alpha player
                            newBoard.setCurrentPlayer(betaPlayer);
                            // Create a new game using the board for the move
                            Game tempGame = new Game(newBoard, true);

                            // Move the piece on the board
                            tempGame.movePiece(destination, origin, selectedPiece);
                            if (tempGame.getCurrentPlayer().getColor().Equals(betaPlayer.getColor()))
                            {
                                tempGame.updateCurrentPlayer();
                            }
                            newBoard.setCurrentPlayer(tempGame.getCurrentPlayer());

                            moveTuple = new Tuple<Square, Piece, Square>(sq, sq.getPiece(), m);

                            AddChild(newBoard, moveTuple);
                            newBoard = null;
                            tempGame = null;
                        }
                    }
                }
            }
            catch(NullReferenceException e)
            {
                //
            }
            
        }

        /// <summary>
        /// <see cref="ToString"/> override
        /// </summary>
        /// <returns>A <see cref="String"/> represntation of this <see cref="Node"/></returns>
        public override string ToString()
        {
            string theString = "";

            theString += "\n | Score     : " + score + " ";
            theString += "\n | Visits    : " + visitCount;
            theString += "\n | Has Won   : " + hasWon;
            theString += "\n | Children  : " + children.Count;
            theString += "\n | UCT Value : " + UCTSelectBest();

            return theString;
        }

    }
}
