using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace VikingChess.Model
{
    /// <summary>
    /// The <see cref="Game"/> class contains the methods needed to process moves.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The Attacking Player
        /// </summary>
        private Player player1;
        /// <summary>
        /// The Defending Player
        /// </summary>
        private Player player2;
        /// <summary>
        /// The current turn count;
        /// </summary>
        private int turnCount = 0;
        /// <summary>
        /// The current player
        /// </summary>
        private Player currentPlayer;
        /// <summary>
        /// A <see cref="String"/> to store the move notation
        /// </summary>
        private string moveNotation = "";
        /// <summary>
        /// A <see cref="bool"/> representing if a piece has been captured or not.
        /// </summary>
        private Boolean pieceCaptured = false;
        /// <summary>
        /// The number of white pieces
        /// </summary>
        private int whitePieces = 0;
        /// <summary>
        /// The number of black pieces
        /// </summary>
        private int blackPieces = 0;
        /// <summary>
        /// The piece (if any) that has been captured.
        /// </summary>
        private Piece capturedPiece;
        /// <summary>
        /// A <see cref="bool"/> representing if the king has escaped or not.
        /// </summary>
        private bool kingEscaped = false;
        /// <summary>
        /// A <see cref="bool"/> representing if the king has been captured or not.
        /// </summary>
        private bool kingCaptured = false;
        /// <summary>
        /// A <see cref="bool"/> representing if the <see cref="Game"/> is over or not.
        /// </summary>
        private bool gameOver = false;
        /// <summary>
        /// The <see cref="Board"/> to use for this <see cref="Game"/>
        /// </summary>
        private Board board;
        /// <summary>
        /// A <see cref="bool"/> representing if the <see cref="Game"/> is being simulated or not.
        /// </summary>
        private bool simulated = false;
        /// <summary>
        /// A <see cref="string"/> defining the type of <see cref="Game"/>.
        /// </summary>
        private string gameType = null;
        /// <summary>
        /// The score for this <see cref="Game"/>
        /// </summary>
        private int score = 0;
        /// <summary>
        /// Contains all previous <see cref="Board"/> objects so that a move can be undone.
        /// </summary>
        private Dictionary<int, Board> stateHistory = new Dictionary<int, Board>();

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public Game()
        {

        }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use for the <see cref="Game"/>.</param>
        public Game(Board board)
        {
            player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.HUMAN);
            player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.HUMAN);
            currentPlayer = player1;
            board.setCurrentPlayer(currentPlayer);
            this.board = board;
            this.blackPieces = board.countPieces(Enums.Color.BLACK);
            this.whitePieces = board.countPieces(Enums.Color.BLACK);
            Board newState = new Board(board);
            stateHistory.Add(turnCount, newState);
        }

        /// <summary>
        /// Constructor for simulated games. This is used only for the <see cref="MCTS"/> algorithm.
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use for the <see cref="Game"/>.</param>
        /// <param name="simulated"><see cref="simulated"/></param>
        public Game(Board board, bool simulated)
        {
            this.simulated = simulated;
            player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.CPU);
            player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.CPU);
            if(board.getCurrentPlayer().getColor().Equals(player1.getColor()))
            {
                currentPlayer = player1;
                
            }
            else 
            {
                currentPlayer = player2;
            }
            this.board = board;
            blackPieces = board.countPieces(Enums.Color.BLACK);
            whitePieces = board.countPieces(Enums.Color.WHITE);
            Board newState = new Board(board);
            stateHistory.Add(turnCount, newState);
        }

        /// <summary>
        /// The Main Constructor
        /// </summary>
        /// <param name="board">The <see cref="Board"/> to use for the <see cref="Game"/>.</param>
        /// <param name="parameters">A <see cref="Tuple{T1, T2, T3, T4, T5}"/> containing the setup parameters for this game</param>
        public Game(Board board, Tuple<string, string, string, string, string> parameters)
        {
            if (parameters.Item2.Equals("ATTACKER"))
            {
                if (parameters.Item1.Equals("HUMAN"))
                {
                    player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.HUMAN);
                    player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.HUMAN);
                }
                else if (parameters.Item1.Equals("CPU"))
                {
                    player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.HUMAN);
                    player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.CPU);
                }
            }
            else if (parameters.Item2.Equals("DEFENDER"))
            {
                if (parameters.Item1.Equals("HUMAN"))
                {
                    player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.HUMAN);
                    player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.HUMAN);
                }
                else if (parameters.Item1.Equals("CPU"))
                {
                    player1 = new Player(Enums.Color.BLACK, Enums.PlayerType.CPU);
                    player2 = new Player(Enums.Color.WHITE, Enums.PlayerType.HUMAN);
                }
            }
            if(parameters.Item3 != "")
            {
                player1.setName(parameters.Item3);
            }
            else
            {
                if (player1.getType().Equals(Enums.PlayerType.CPU))
                {
                    player1.setName("CPU");
                }
                else
                {
                    player1.setName("Player 1");
                }
                
            }
            if(parameters.Item4 != "")
            {
                player2.setName(parameters.Item4);
            }
            else
            {
                if (player2.getType().Equals(Enums.PlayerType.CPU))
                {
                    player2.setName("CPU");
                }
                else
                {
                    player2.setName("Player 2");
                }
            }
            gameType = parameters.Item5;
            currentPlayer = player1;
            board.setCurrentPlayer(currentPlayer);
            this.board = board;
            blackPieces = board.countPieces(Enums.Color.BLACK);
            whitePieces = board.countPieces(Enums.Color.WHITE);
            Board newState = new Board(board);
            stateHistory.Add(turnCount, newState);
        }

        /// <summary>
        /// Sets the <see cref="currentPlayer"/> for this <see cref="Game"/>
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to set as the <see cref="currentPlayer"/>.</param>
        public void setCurrentPlayer(Player player)
        {
            currentPlayer = player;
        }

        /// <summary>
        /// Sets the <see cref="score"/> for this <see cref="Game"/>
        /// </summary>
        /// <param name="score"></param>
        public void setScore(int score)
        {
            this.score = score;
        }

        /// <summary>
        /// Returns the <see cref="Board"/> of this game.
        /// </summary>
        /// <returns><see cref="board"/></returns>
        public Board getBoard()
        {
            return board;
        }

        /// <summary>
        /// Updates the <see cref="currentPlayer"/> of this <see cref="Game"/>
        /// </summary>
        public void updateCurrentPlayer()
        {
            if(currentPlayer == player1)
            {
                currentPlayer = player2;
                board.setCurrentPlayer(player2);
            }
            else
            {
                currentPlayer = player1;
                board.setCurrentPlayer(player1);
            }
        }

        /// <summary>
        /// Returns the <see cref="currentPlayer"/> of this <see cref="Game"/>
        /// </summary>
        /// <returns><see cref="currentPlayer"/></returns>
        public Player getCurrentPlayer()
        {
            return currentPlayer;
        }

        /// <summary>
        /// Get the <see cref="player1"/> <see cref="Player"/>
        /// </summary>
        /// <returns><see cref="player1"/></returns>
        public Player getPlayer1()
        {
            return player1;
        }

        /// <summary>
        /// Get the <see cref="player2"/> <see cref="Player"/>
        /// </summary>
        /// <returns><see cref="player2"/></returns>
        public Player getPlayer2()
        {
            return player2;
        }

        /// <summary>
        /// Returns the <see cref="turnCount"/> of this <see cref="Game"/>
        /// </summary>
        /// <returns><see cref="turnCount"/></returns>
        public int getTurnCount()
        {
            return turnCount;
        }

        /// <summary>
        /// Move a <see cref="Piece"/> on the <see cref="board"/>
        /// </summary>
        /// <param name="destination">The destination <see cref="Square"/> to move the <see cref="Piece"/> to.</param>
        /// <param name="origin">The origin <see cref="Square"/> to move the <see cref="Piece"/> from.</param>
        /// <param name="selectedPiece">The selected <see cref="Piece"/> to move.</param>
        public void movePiece(Square destination, Square origin, Piece selectedPiece)
        {
            // If there is a valid piece
            if (selectedPiece != null && (selectedPiece.getColor() == currentPlayer.getColor()))
            {
                pieceCaptured = false;
                capturedPiece = null;
                List<Square> selectedPieceMoves = selectedPiece.generateLegalMoves(board);
                foreach (Square move in selectedPieceMoves)
                {
                    Square m = move;

                    // If the destination square is a valid move
                    if (destination.getFile().Equals(m.getFile()) && destination.getRank().Equals(m.getRank()))
                    {
                        // Remove the piece from the origin square
                        origin.setPiece(null);
                        // Set the location of the selected piece to the location of the destination square
                        selectedPiece.setLocation(destination.getRank(), destination.getFile());
                        // Set the destination square's piece to the selected piece
                        destination.setPiece(selectedPiece);
                        // Check if any captures occured
                        capturePiece(destination, selectedPiece);
                        checkKingMoves(selectedPiece, destination);
                        updateTurnCount();
                        // If a piece has been captured
                        if (pieceCaptured)
                        {
                            if(!simulated)
                            {
                                PlaySound("piece_captured.wav");
                            }
                            // Reflect this in the move notation (e.g. A8-B8xC8)
                            moveNotation = turnCount + ". " + origin.getFile() + (int)origin.getRank() + "-" + destination.getFile() + (int)destination.getRank() + "x" + capturedPiece.getFile() + (int)capturedPiece.getRank() + "\n";
                        }
                        // Otherwise, just output the standard notation (e.g. A8 - B8)
                        else
                        {
                            if (!simulated)
                            {
                                PlaySound("piece_place.wav");
                            }
                            moveNotation = turnCount + ". " + origin.getFile() + (int)origin.getRank() + "-" + destination.getFile() + (int)destination.getRank() + "\n";
                        }
                        if (!simulated)
                        {
                            currentPlayer.updateMoveList(moveNotation);
                        }
                        updateCurrentPlayer();
                        Board newState = new Board(board);
                        stateHistory.Add(turnCount, newState);
                    }
                }
                
            }
        }

        /// <summary>
        /// Move a <see cref="Piece"/> on the <see cref="board"/>
        /// </summary>
        /// <param name="destination">The destination <see cref="Square"/> to move the <see cref="Piece"/> to.</param>
        /// <param name="origin">The origin <see cref="Square"/> to move the <see cref="Piece"/> from.</param>
        /// <param name="selectedPiece">The selected <see cref="Piece"/> to move.</param>
        /// <returns><see cref="board"/></returns>
        public Board movePieceAndGetBoard(Square destination, Square origin, Piece selectedPiece)
        {
            // If there is a valid piece
            if (selectedPiece != null && (selectedPiece.getColor() == currentPlayer.getColor()))
            {
                pieceCaptured = false;
                capturedPiece = null;
                List<Square> selectedPieceMoves = selectedPiece.generateLegalMoves(board);
                foreach (Square move in selectedPieceMoves)
                {
                    Square m = move;

                    // If the destination square is a valid move
                    if (destination.getFile().Equals(m.getFile()) && destination.getRank().Equals(m.getRank()))
                    {
                        // Remove the piece from the origin square
                        origin.setPiece(null);
                        // Set the location of the selected piece to the location of the destination square
                        selectedPiece.setLocation(destination.getRank(), destination.getFile());
                        // Set the destination square's piece to the selected piece
                        destination.setPiece(selectedPiece);
                        // Check if any captures occured
                        capturePiece(destination, selectedPiece);
                        checkKingMoves(selectedPiece, destination);
                        updateTurnCount();
                        // If a piece has been captured
                        if (pieceCaptured)
                        {
                            if (!simulated)
                            {
                                PlaySound("piece_captured.wav");
                            }
                            // Reflect this in the move notation (e.g. A8-B8xC8)
                            moveNotation = turnCount + ". " + origin.getFile() + (int)origin.getRank() + "-" + destination.getFile() + (int)destination.getRank() + "x" + capturedPiece.getFile() + (int)capturedPiece.getRank() + "\n";
                        }
                        // Otherwise, just output the standard notation (e.g. A8 - B8)
                        else
                        {
                            if (!simulated)
                            {
                                PlaySound("piece_place.wav");
                            }
                            moveNotation = turnCount + ". " + origin.getFile() + (int)origin.getRank() + "-" + destination.getFile() + (int)destination.getRank() + "\n";
                        }
                        if (!simulated)
                        {
                            currentPlayer.updateMoveList(moveNotation);
                        }
                        updateCurrentPlayer();
                        Board newState = new Board(board);
                        stateHistory.Add(turnCount, newState);
                    }
                }
            }
            return board;
        }

        /// <summary>
        /// Update the <see cref="turnCount"/> for this <see cref="Game"/>
        /// </summary>
        public void updateTurnCount()
        {
            ++turnCount;
        }

        /// <summary>
        /// Checks to see if the King has moved to an edge.
        /// </summary>
        /// <param name="selectedPiece">The <see cref="Piece"/> to check with.</param>
        /// <param name="destination">The <see cref="Square"/> to check against.</param>
        public void checkKingMoves(Piece selectedPiece, Square destination)
        {
            if(selectedPiece.getType() == Enums.PieceType.KING)
            {
                for (int k = 1; k < board.GetSize() - 2; ++k)
                {
                    // Top
                    if (board.GetSquare(1, k).Equals(destination))
                    {
                        kingEscaped = true;
                    }
                    // Left
                    else if (board.GetSquare(k, 1).Equals(destination))
                    {
                        kingEscaped = true;
                    }
                    // Vottom
                    else if (board.GetSquare(board.GetSize() - 2, k).Equals(destination))
                    {
                        kingEscaped = true;
                    }
                    // Right
                    else if (board.GetSquare(k, board.GetSize() - 2).Equals(destination))
                    {
                        kingEscaped = true;
                    }
                }
            }
        }

        /// <summary>
        /// Undo the last move
        /// </summary>
        /// <returns>The previous <see cref="Board"/> obtained from the <see cref="stateHistory"/></returns>
        public Board undoMove()
        {
            if(turnCount > 0)
            {
                stateHistory.Remove(turnCount);
                --turnCount;
                moveNotation = moveNotation.Remove(moveNotation.Count() - 1);
                updateCurrentPlayer();
                currentPlayer.removeFromMoveList();
                board = new Board(stateHistory[turnCount]);
            }
            else
            {
                //return History[turnCount];
            }
            return board;
                
        }

        /// <summary>
        /// Checks to see if a <see cref="Player"/> has won or not.
        /// </summary>
        /// <returns><see cref="gameOver"/></returns>
        public bool checkWin()
        {
            // If the king has been captured, player 1 wins.
            if (kingCaptured)
            {
                player1.setWin(true);
                gameOver = true;
            }
            // If the king has escaped, player 2 wins.
            else if (kingEscaped)
            {
                player2.setWin(true);
                gameOver = true;
            }
            return gameOver;
        }

        /// <summary>
        /// Checks to see if the king is surrounded
        /// </summary>
        /// <param name="kingSquare">The <see cref="Square"/> that the king <see cref="Piece"/> is on.</param>
        /// <returns>A <see cref="bool"/> representing if the king is surrounded or not.</returns>
        public bool checkKingCapture(Square kingSquare)
        {
            Boolean isNull = true;
            Square[] kingNeighbours = kingSquare.getNeighbours();

            // Ensure that there is a piece beside the king in every direction.
            if(kingNeighbours[0] != null && kingNeighbours[0].getPiece() != null && 
                kingNeighbours[1] != null && kingNeighbours[1].getPiece() != null && 
                kingNeighbours[2] != null && kingNeighbours[2].getPiece() != null && 
                kingNeighbours[3] != null && kingNeighbours[3].getPiece() != null)
            {
                isNull = false;
            }
            else
            {
                isNull = true;
            }
         
            // If there is a piece beisde the king in every direction and they are all a different color to the king, the king can be captured.
            if((!isNull) && 
               kingNeighbours[0].getPiece().getColor() != kingSquare.getPiece().getColor() && 
               kingNeighbours[1].getPiece().getColor() != kingSquare.getPiece().getColor() && 
               kingNeighbours[2].getPiece().getColor() != kingSquare.getPiece().getColor() && 
               kingNeighbours[3].getPiece().getColor() != kingSquare.getPiece().getColor())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if there is a <see cref="Piece"/> that can be captured in the passed in direction.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <param name="neighbourSquares">The neighbouring <see cref="Square"/> <see cref="Array"/></param>
        /// <param name="selectedPiece">The <see cref="Piece"/> to check with.</param>
        public void checkCapture(int direction, Square[] neighbourSquares, Piece selectedPiece)
        {
            // Check for a Left Capture, ensure that there is a square to the left.
            if (neighbourSquares[direction] != null)
            {
                // Get the neighbours of the square
                Square[] NeighboursNeighbours = neighbourSquares[direction].getNeighbours();
                // Get the neighbour of the destination square in the specified direction
                Square neighbour = neighbourSquares[direction];
                // Ensure that the square is not null
                if (NeighboursNeighbours[direction] != null)
                {
                    // Get the neighbour of the nieghbour square
                    Square neighboursNeighbour = NeighboursNeighbours[direction];

                    // If the square contains a piece and it is a different color to the piece in the destination square
                    if ((neighbour.getPiece() != null) && (neighbour.getPiece().getType() != Enums.PieceType.KING) && (neighbour.getPiece().getColor() != selectedPiece.getColor()))
                    {
                        // If the nneighbour's neighbour square has a piece and it is the same color as the piece in the destination square
                        if ((neighboursNeighbour.getPiece() != null) && (neighboursNeighbour.getPiece().getColor() == selectedPiece.getColor()))
                        {
                            capturedPiece = neighbour.getPiece();
                            if (capturedPiece.getColor().Equals(player1.getColor()))
                            {
                                blackPieces--;
                                board.decrementBlackCount();
                            }
                            else
                            {
                                whitePieces--;
                                board.decrementWhiteCount();
                            }
                            // remove the piece from the board
                            neighbour.setPiece(null);
                            // Update the board
                            pieceCaptured = true;
                        }
                        // Otherwise, if the neighbour's square is a corner square or a throne square,
                        else if ((neighboursNeighbour.getPiece() == null) && ((neighboursNeighbour.getSquareType() == Enums.SquareType.CORNER) || (neighboursNeighbour.getSquareType() == Enums.SquareType.THRONE)))
                        {
                            capturedPiece = neighbour.getPiece();
                            if (capturedPiece.getColor() == player1.getColor())
                            {
                                blackPieces--;
                            }
                            else
                            {
                                whitePieces--;
                            }

                            // remove the piece on the left square
                            neighbour.setPiece(null);
                            // Update the board
                            pieceCaptured = true;
                        }
                    }
                    // If the neighbour contains a King
                    else if (neighbourSquares[direction].getPiece() != null && neighbourSquares[direction].getPiece().getType() == Enums.PieceType.KING)
                    {
                        // If the king is surrounded
                        if (checkKingCapture(neighbourSquares[direction]))
                        {
                            capturedPiece = neighbourSquares[direction].getPiece();

                            neighbourSquares[direction].setPiece(null);
                            pieceCaptured = true;
                            kingCaptured = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loops through each of the nieghbours of the destination <see cref="Square"/> and checks if any piece has been captured.
        /// </summary>
        /// <param name="destination">The <see cref="Square"/> to check the neighbours of.</param>
        /// <param name="selectedPiece">The <see cref="Piece"/> capturing.</param>
        public void capturePiece(Square destination, Piece selectedPiece)
        {
            // Get the neighbours of the destination square
            Square[] neighbourSquares = destination.getNeighbours();

            for(int i = 0; i < 4; ++i)
            {
                checkCapture(i, neighbourSquares, selectedPiece);
            }
        }

        /// <summary>
        /// Plays a sound
        /// </summary>
        /// <param name="wavName">The name of the soundfile to play.</param>
        public async void PlaySound(string wavName)
        {
            MediaElement sound = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets\\Sounds");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(wavName);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            sound.SetSource(stream, file.ContentType);
            sound.Play();
            await Task.Delay(TimeSpan.FromSeconds(.5));
        }
    }
}
