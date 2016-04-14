using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VikingChess.Model
{

    /// <summary>
    /// The MCTS Class contains the implementation for the Monte Carlo Tree Search algorithm. This implementation
    /// uses the UCT formula for the selection step after all initial nodes for the CPU player have been generated.
    /// This formula is defined in the <see cref="Node"/> class which holds data about the current <see cref="Board"/>
    /// state.
    /// </summary>
    class MCTS
    {
        /// <summary>
        /// The number of points a node receives if the simulation results in a win for the CPU player.
        /// </summary>
        private const double WIN_POINTS = 100.0;
        /// <summary>
        /// The number of points a node receives if the simulation results in a loss for the CPU player.
        /// </summary>
        private const double LOSS_POINTS = -100;
        /// <summary>
        /// The number of times the steps of the MCTS algorithm run before the best node is chosen. 
        /// Increasing this value may result in a better decision by the CPU at the cost of an 
        /// increase in the time taken to produce a result and a higher memory cost.
        /// </summary>
        private const double PLAYOUTS = 1000;
        /// <summary>
        /// The number of moves to process during the simulation step.
        /// </summary>
        private const int SIM_TURNCOUNT = 150;
        /// <summary>
        /// The alphaPlayer is the CPU player running the MCTS algorithm and score are generated in favour of this player.
        /// </summary>
        private Player alphaPlayer;
        /// <summary>
        /// The root node of the game tree.
        /// </summary>
        private Node root;

        /// <summary>
        /// Constructor for the MCTS class
        /// </summary>
        /// <param name="board"><The <see cref="Board"/> to use as a base for this <see cref="MCTS"/> instance</param>
        public MCTS(Board board)
        {
            Board mctsBoard = new Board(board);
            root = new Node(mctsBoard);
            alphaPlayer = new Player(mctsBoard.getCurrentPlayer());
            //generateNodes(mctsBoard);
        }

        /// <summary>
        /// Generate all possible move states for the alpha player.
        /// </summary>
        /// <param name="board">The base <see cref="Board"/> to use when generating nodes.</param>
        public void generateNodes(Board board)
        {
            alphaPlayer = new Player(board.getCurrentPlayer());
            Board tempBoard = new Board(board);

            // Get all pieces that can be moved by the current player
            List<Square> squareList = alphaPlayer.GetAllPieceSquares(tempBoard);
            Tuple<Square, Piece, Square> moveTuple = null;
            Game newGame = null;
            Board newBoard = null;

            if (alphaPlayer.getColor().Equals(Enums.Color.BLACK))
            {
                moveTuple = alphaPlayer.captureKing(tempBoard, squareList);
                if(moveTuple != null)
                {
                    // Create a new game using the board for the move
                    newGame = new Game(tempBoard, true);
                    // Move the piece on the board
                    newGame.movePiece(moveTuple.Item3, moveTuple.Item1, moveTuple.Item2);
                    if (newGame.getCurrentPlayer().getColor().Equals(Enums.Color.WHITE))
                    {
                        newGame.updateCurrentPlayer();
                    }
                    // Set the current player of the game to the alpha player
                    tempBoard.setCurrentPlayer(newGame.getCurrentPlayer());
                    root.AddChild(tempBoard, moveTuple);
                }
            }
            else if (alphaPlayer.getColor().Equals(Enums.Color.WHITE))
            {
                moveTuple = alphaPlayer.escapeKing(tempBoard, squareList);
                if(moveTuple != null)
                {
                    // Create a new game using the board for the move
                    newGame = new Game(tempBoard, true);
                    // Move the piece on the board
                    newGame.movePiece(moveTuple.Item3, moveTuple.Item1, moveTuple.Item2);
                    if (newGame.getCurrentPlayer().getColor().Equals(Enums.Color.WHITE))
                    {
                        newGame.updateCurrentPlayer();
                    }
                    // Set the current player of the game to the alpha player
                    tempBoard.setCurrentPlayer(newGame.getCurrentPlayer());
                    root.AddChild(tempBoard, moveTuple);
                }
            }
            else
            {
                // Loop through the list of squares
                foreach (Square sq in squareList.ToArray())
                {
                    if (sq.getPiece() != null)
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
                            newBoard = new Board(board);

                            // Create a new game using the board for the move
                            newGame = new Game(newBoard, true);

                            // Move the piece on the board
                            newGame.movePiece(destination, origin, selectedPiece);
                            if (newGame.getCurrentPlayer().getColor().Equals(Enums.Color.WHITE))
                            {
                                newGame.updateCurrentPlayer();
                            }
                            // Set the current player of the game to the alpha player
                            newBoard.setCurrentPlayer(newGame.getCurrentPlayer());
                            moveTuple = new Tuple<Square, Piece, Square>(sq, sq.getPiece(), destination);

                            root.AddChild(newBoard, moveTuple);
                            newBoard = null;
                            newGame = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs the MCTS algorithm.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to perform the algorithm on.</param>
        /// <returns>The best <see cref="Node"/> to obtain a move from.</returns>
        public Node makeMove(Board board)
        {
            for(int i=0; i < PLAYOUTS; ++i)
            {
                PerformSteps();
            }

            Node bestNode = root.getBestChild();

            return bestNode;
        }

        /// <summary>
        /// Performs te four steps of the MCTS algorithm:
        /// 1. Selection.
        /// 2. Expansion.
        /// 3. Simulation.
        /// 4. Backpropogation
        /// </summary>
        public void PerformSteps()
        {
            // Step 1. Selection
            List<Node> visitedNodes = new List<Node>();
            Node curr = root;
            Node bestChild = null;
            double score = 0;
            visitedNodes.Add(curr);

            // Find the best child node to expand
            while (curr.isLeaf().Equals(false))
            {
                curr = curr.UCTSelectBest();
                visitedNodes.Add(curr);
            }
            if (curr.getWin().Equals(false))
            {
                // Step 2. Expansion
                curr.ExpandNode();
                bestChild = curr.UCTSelectBest();
            }
            if (bestChild == null)
            {
                // Best child is terminal state
                bestChild = curr;
            }
            else
            {
                visitedNodes.Add(bestChild);
            }
            if (bestChild.getWin().Equals(false))
            {
                // Step 3. Simulation
                score = SimulateGame(bestChild);
            }

            // Step 4. Backpropogation
            foreach(Node n in visitedNodes)
            {
                n.updateStats(score);
            }
        }

        /// <summary>
        /// Simulates a <see cref="Game"/> between two <see cref="Player"/> then scores the resulting <see cref="Board"/>.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to simulate the <see cref="Game"/> from.</param>
        /// <returns>The <see cref="Node.score"/> for this simulation/></returns>
        public double SimulateGame(Node node)
        {
            Board gameBoard = new Board(node.GetBoard());
            Game game = new Game(gameBoard, true);

            try
            {
                // Then for the specified turn count, play out the game.
                while (game.getTurnCount() < SIM_TURNCOUNT)
                {
                    if (game.checkWin())
                    {
                        node.setWin(true);
                        return evaluateBoard(gameBoard, game);
                    }

                    Tuple<Square, Piece, Square> cpuMovesTuple = null;
                    // Try to find a winning move.
                    cpuMovesTuple = game.getCurrentPlayer().GetWinningMove(gameBoard);
                    // If no winning move was found
                    if (cpuMovesTuple == null)
                    {
                        // Try to capture a piece
                        cpuMovesTuple = game.getCurrentPlayer().capturePiece(gameBoard);
                    }
                    if (cpuMovesTuple == null)
                    {
                        // Find a general move
                        cpuMovesTuple = game.getCurrentPlayer().getMoveTuple(gameBoard);
                    }
                    Piece selectedCPUPiece = cpuMovesTuple.Item2;
                    Square destination = cpuMovesTuple.Item3;
                    Square origin = cpuMovesTuple.Item1;

                    // Move the piece
                    game.movePieceAndGetBoard(destination, origin, selectedCPUPiece);
                    //game.updateCurrentPlayer();
                }

                return evaluateBoard(gameBoard, game);
            }
            catch(NullReferenceException e)
            {
                return 0;
            }
            
        }

        /// <summary>
        /// Scores a <see cref="Board"/> on the following parameters: Win/Loss, <see cref="Piece"/> count and <see cref="Board"/> Control.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to score for.</param>
        /// <param name="game">The <see cref="Game"/> the <see cref="Board"/> was used in.</param>
        /// <returns>A score for the <see cref="Board"/> that was passed in.</returns>
        private double evaluateBoard(Board board, Game game)
        {
            double score = 0;

            // If player 1 has won and it is the alpha player
            if (game.getPlayer1().getWin() && alphaPlayer.getColor().Equals(Enums.Color.BLACK))
            {
                // Score positive
                score += WIN_POINTS;

                // Get a count of pieces
                score += scorePieces(board, game);

                // Evaluate control of rank and file
                score += evaluateControl(board, game);
            }
            // Else if player 2 has won and it is the alpha player
            else if (game.getPlayer2().getWin() && alphaPlayer.getColor().Equals(Enums.Color.WHITE))
            {
                // Score positive
                score += WIN_POINTS;

                // Get a count of pieces
                score += scorePieces(board, game);

                // Evaluate control of rank and file
                score += evaluateControl(board, game);
            }
            // Else if player 1 has won and it is not the alpha player
            else if (game.getPlayer1().getWin() && alphaPlayer.getColor().Equals(Enums.Color.WHITE))
            {
                // Score negative
                score += LOSS_POINTS;

                // Get a count of pieces
                score += scorePieces(board, game);

                // Evaluate control of rank and file
                score += evaluateControl(board, game);
            }
            // Else if player 2 has won and it is not the alpha player
            else if (game.getPlayer2().getWin() && alphaPlayer.getColor().Equals(Enums.Color.BLACK))
            {
                // Score negative
                score += LOSS_POINTS;

                // Get a count of pieces
                score += scorePieces(board, game);

                // Evaluate control of rank and file
                score += evaluateControl(board, game);
            }
            else
            {
                // Get a count of pieces
                score += scorePieces(board, game);

                // Evaluate control of rank and file
                score += evaluateControl(board, game);
            }

            // Check the king's proximity to the edges
            return score;
        }

        /// <summary>
        /// Finds the outermost pieces on the board and scores the board appropriately. 
        /// The reason for this is that control of the outer squares in an advantage to both players.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to score for.</param>
        /// <param name="game">The <see cref="Game"/> the <see cref="Board"/> was used in.</param>
        /// <returns>A score for the <see cref="Board"/> that was passed in.</returns>
        private double evaluateControl(Board board, Game game)
        {
            Piece leftmostPiece = null;
            Piece rightmostPiece = null;
            Piece topmostPiece = null;
            Piece bottommostPiece = null;
            double blackScore = 0;
            double whiteScore = 0;
            double score = 0;
            int i;
            int j;

            // Check the files for outermost pieces
            for (i = 1; i < board.GetSize()-2; ++i)
            {
                for(j = 1; j < board.GetSize()-2; ++j)
                {
                    if(board.GetSquare(i,j) != null)
                    {
                        if(board.GetSquare(i,j).getPiece() != null)
                        {
                            // On the first iteration, set all pieces
                            if(leftmostPiece == null)
                            {
                                leftmostPiece = board.GetSquare(i, j).getPiece();
                                rightmostPiece = board.GetSquare(i, j).getPiece();
                            }
                            // If the current piece's file is less than the current leftmostPiece, make it the leftmostPiece
                            if (board.GetSquare(i, j).getPiece().getFile() < leftmostPiece.getFile())
                            {
                                leftmostPiece = board.GetSquare(i, j).getPiece();
                            }
                            // If the current piece's file is greater than the current rightmostPiece, make it the rightmostPiece
                            if (board.GetSquare(i, j).getPiece().getFile() > rightmostPiece.getFile())
                            {
                                rightmostPiece = board.GetSquare(i, j).getPiece();
                            }
                        }
                    }
                }
                if(leftmostPiece != null)
                {
                    // Score for leftmostPiece
                    if (leftmostPiece.getColor().Equals(Enums.Color.BLACK))
                    {
                        ++blackScore;
                    }
                    else if (leftmostPiece.getColor().Equals(Enums.Color.WHITE))
                    {
                        ++whiteScore;
                    }
                }
                if(rightmostPiece != null)
                {
                    // Score for rightmostPiece
                    if (rightmostPiece.getColor().Equals(Enums.Color.BLACK))
                    {
                        ++blackScore;
                    }
                    else if (rightmostPiece.getColor().Equals(Enums.Color.WHITE))
                    {
                        ++whiteScore;
                    }
                }
                leftmostPiece = null;
                rightmostPiece = null;
            }
            // Check the ranks for outermost pieces
            for (i = 1; i < board.GetSize() - 2; ++i)
            {
                for (j = 1; j < board.GetSize() - 2; ++j)
                {
                    if (board.GetSquare(j, i) != null)
                    {
                        if (board.GetSquare(j, i).getPiece() != null)
                        {
                            // On the first iteration, set all pieces
                            if (topmostPiece == null)
                            {
                                topmostPiece = board.GetSquare(j, i).getPiece();
                                bottommostPiece = board.GetSquare(j, i).getPiece();
                            }
                            // If the current piece's file is less than the current leftmostPiece, make it the leftmostPiece
                            if (board.GetSquare(j, i).getPiece().getFile() < topmostPiece.getFile())
                            {
                                topmostPiece = board.GetSquare(j, i).getPiece();
                            }
                            // If the current piece's file is greater than the current rightmostPiece, make it the rightmostPiece
                            if (board.GetSquare(j, i).getPiece().getFile() > bottommostPiece.getFile())
                            {
                                bottommostPiece = board.GetSquare(j, i).getPiece();
                            }
                        }
                        
                    }
                }
                if(topmostPiece != null)
                {
                    // Score for topmostPiece
                    if (topmostPiece.getColor().Equals(Enums.Color.BLACK))
                    {
                        ++blackScore;
                    }
                    else if (topmostPiece.getColor().Equals(Enums.Color.WHITE))
                    {
                        ++whiteScore;
                    }
                }
                if(bottommostPiece != null)
                {
                    // Score for bottommostPiece
                    if (bottommostPiece.getColor().Equals(Enums.Color.BLACK))
                    {
                        ++blackScore;
                    }
                    else if (bottommostPiece.getColor().Equals(Enums.Color.WHITE))
                    {
                        ++whiteScore;
                    }
                }
                topmostPiece = null;
                bottommostPiece = null;
            }

            // score for the alphaPlayer
            if (alphaPlayer.getColor().Equals(Enums.Color.BLACK))
            {
                score += blackScore;
                score -= whiteScore;
            }
            else if (alphaPlayer.getColor().Equals(Enums.Color.WHITE))
            {
                score += whiteScore;
                score -= blackScore;
            }

            return score;
        }

        /// <summary>
        /// Count the number of pieces on the board and score the board appropriately
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to score for.</param>
        /// <param name="game">The <see cref="Game"/> the <see cref="Board"/> was used in.</param>
        /// <returns>A score for the <see cref="Board"/> that was passed in.</returns>
        private int scorePieces(Board board, Game game)
        {
            int score = 0;

            int blackCount = board.countPieces(Enums.Color.BLACK);
            int whiteCount = board.countPieces(Enums.Color.WHITE);

            // If the AlphaPlayer is Attacking
            if (alphaPlayer.getColor().Equals(Enums.Color.BLACK))
            {
                // Reflect the importance of a low defender count by penalizing boards with a high defender count
                score += (blackCount);
                score -= (whiteCount);
            }
            // If the AlphaPlayer is Defending
            else if (alphaPlayer.getColor().Equals(Enums.Color.WHITE))
            {
                // Reflect the importance of a high defender count by rewarding boards with a high defender count;
                score -= (blackCount);
                score += (whiteCount);
            }

            return score;
        }
    }
}
