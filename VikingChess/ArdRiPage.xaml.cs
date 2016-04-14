using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Imaging;

using VikingChess.Model;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

namespace VikingChess
{
    /// <summary>
    /// The page responsible for displaying and controlling the Ard Rí gametype
    /// </summary>
    public sealed partial class ArdRiPage : Page
    {
        private Brush stationaryStrokeBrush;
        private Brush transformingStrokeBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x13, 0x01));
        private double strokeThickness;
        private double transformingStrokeThickness = 2;
        private ImageBrush blackImageBrush = new ImageBrush();
        private ImageBrush whiteImageBrush = new ImageBrush();
        private ImageBrush kingImageBrush = new ImageBrush();
        private Board board = new Board(7, "Ard Ri");
        private Rectangle[] rectArr;
        private Rectangle lastTappedRect;
        private Game game;
        private List<String> p1MoveList;
        private List<String> p2MoveList;
        private bool checkMate = false;
        private Tuple<string, string, string, string, string> parameters;
        private Tuple<Square, Piece, Square> cpuMovesTuple;
        private Board mctsBoard;

        private double msCount = 0;
        private double secondCount = 0;
        private double minuteCount = 0;

        private double p_msCount = 0;
        private double p_secondCount = 0;
        private double p_minuteCount = 0;

        private double c_msCount = 0;
        private double c_secondCount = 0;
        private double c_minuteCount = 0;

        private DateTime gameStarted;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DateTime playerOneStarted = DateTime.UtcNow;
        private DispatcherTimer playerOneTimer = new DispatcherTimer();
        private DateTime playerTwoStarted = DateTime.UtcNow;
        private DispatcherTimer playerTwoTimer = new DispatcherTimer();

        private BackgroundWorker bw = new BackgroundWorker();

        /// <summary>
        /// Constructor for ArdRiPage. Initializes the component, background worker and timers
        /// </summary>
        public ArdRiPage()
        {
            InitializeComponent();

            // Initalize the background worker.
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            // Initialize Game Timer
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); ;
            gameTimer.Tick += new EventHandler<object>(gameTimer_Tick);

            // Initialize Player 1 timer
            playerOneTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); ;
            playerOneTimer.Tick += new EventHandler<object>(playerOneTimer_Tick);

            // Initialize Player 2 timer
            playerTwoTimer.Interval = new TimeSpan(0, 0, 0, 0, 1); ;
            playerTwoTimer.Tick += new EventHandler<object>(playerTwoTimer_Tick);

            // Set the Start time of the Game Timer and Player 1 Timer
            gameStarted = DateTime.UtcNow;
            playerOneStarted = DateTime.UtcNow;

            // Start the Game Timer and Player 1 Timer
            gameTimer.Start();
            playerOneTimer.Start();
        }

        /// <summary>
        /// Sets up the base parameters for the game
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // get configuration and create a new game with this configuration
            parameters = e.Parameter as Tuple<string, string, string, string, string>;
            game = new Game(board, parameters);

            // Set Player names
            player1Text.Text = game.getPlayer1().getName();
            player2Text.Text = game.getPlayer2().getName();

            // Hide the scrollbars for the movelists
            scrollViewerLeft.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            scrollViewerRight.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            // Get all rectangles on the board
            rectArr = new Rectangle[] { A1, A2, A3, A4, A5, A6, A7,
                                        B1, B2, B3, B4, B5, B6, B7, 
                                        C1, C2, C3, C4, C5, C6, C7,
                                        D1, D2, D3, D4, D5, D6, D7,
                                        E1, E2, E3, E4, E5, E6, E7,
                                        F1, F2, F3, F4, F5, F6, F7,
                                        G1, G2, G3, G4, G5, G6, G7
                                        };

            // Apply the event handlers to each rectangle
            for (int i = 0; i < rectArr.Length; ++i)
            {
                rectArr[i].Tapped += rectangle_tapped;
            }

            // Bind the Rectangles to their corresponding squares
            bindRectangles();
            // Render the pieces on screen
            displayPieces(rectArr, board);
            displayMoveNotation();
            notifyPlayer();

            // Make a CPU move if first player is CPU
            if (bw.IsBusy != true)
            {
                mctsBoard = new Board(board);
                bw.RunWorkerAsync();
            }
        }

        /// <summary>
        /// The DoWork method for the <see cref="BackgroundWorker"/>. It runs the MCTS algorithm in the background and
        /// retrieves the moveTuple for the CPU player for their current turn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (bw.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {
                // If the current player is a CPU player then get the move tuple for their current turn.
                if (game.getCurrentPlayer().getType().Equals(Enums.PlayerType.CPU) && game.checkWin() == false)
                {
                    cpuMovesTuple = null;
                    // Make CPU Move
                    cpuMovesTuple = game.getCurrentPlayer().makeMCTSMove(mctsBoard);
                }
            }
        }

        /// <summary>
        /// Runs once the backgroundWorker completes and processes the rest of the CPU player's move.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (game.getCurrentPlayer().getType().Equals(Enums.PlayerType.CPU) && game.checkWin() == false)
            {
                moveCPU(game.getCurrentPlayer(), cpuMovesTuple);
            }
        }

        /// <summary>
        /// Controls the updating and display of the game timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gameTimer_Tick(object sender, object e)
        {
            // If the ms counter reaches 999, reset it and update the second count
            if (msCount >= 999)
            {
                msCount = 0;
                gameStarted = DateTime.UtcNow;
                secondCount++;
                // If the CPU is currently making their move, update the winText to reflect that.
                if (game.getCurrentPlayer() != null && game.getCurrentPlayer().getType().Equals(Enums.PlayerType.CPU))
                {
                    if (winText.Text.Equals(""))
                    {
                        winText.Text = game.getCurrentPlayer().getName() + " is thinking .";
                    }
                    else if (winText.Text.Equals(game.getCurrentPlayer().getName() + " is thinking ."))
                    {
                        winText.Text = game.getCurrentPlayer().getName() + " is thinking . .";
                    }
                    else if (winText.Text.Equals(game.getCurrentPlayer().getName() + " is thinking . ."))
                    {
                        winText.Text = game.getCurrentPlayer().getName() + " is thinking . . .";
                    }
                    else if (winText.Text.Equals(game.getCurrentPlayer().getName() + " is thinking . . ."))
                    {
                        winText.Text = game.getCurrentPlayer().getName() + " is thinking .";
                    }
                }
                else
                {
                    winText.Text = "";
                }
            }
            else
            {
                msCount = (int)(DateTime.UtcNow - gameStarted).TotalMilliseconds;
            }
            if (secondCount == 60)
            {
                secondCount = 0;
                minuteCount++;
            }
            String timeString = minuteCount + " : " + secondCount + " : " + (int)(msCount / 100);
            timer_text.Text = timeString;
        }

        /// <summary>
        /// Controls the updating and display of the player one timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playerOneTimer_Tick(object sender, object e)
        {
            if (p_msCount >= 999)
            {
                p_msCount = 0;
                playerOneStarted = DateTime.UtcNow;
                p_secondCount++;
            }
            else
            {
                p_msCount = (int)(DateTime.UtcNow - playerOneStarted).TotalMilliseconds;
            }
            if (p_secondCount == 60)
            {
                p_secondCount = 0;
                p_minuteCount++;
            }
            String timeString = p_minuteCount + " : " + p_secondCount + " : " + (int)(p_msCount / 100);
            player_timer_text.Text = timeString;
        }

        /// <summary>
        /// Controls the updating and display of the player two timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void playerTwoTimer_Tick(object sender, object e)
        {
            if (c_msCount >= 999)
            {
                c_msCount = 0;
                playerTwoStarted = DateTime.UtcNow;
                c_secondCount++;
            }
            else
            {
                c_msCount = (int)(DateTime.UtcNow - gameStarted).TotalMilliseconds;
            }
            if (c_secondCount == 60)
            {
                c_secondCount = 0;
                c_minuteCount++;
            }
            String timeString = c_minuteCount + " : " + c_secondCount + " : " + (int)(c_msCount / 100);
            cpu_timer_text.Text = timeString;
        }

        /// <summary>
        /// Resets the PlayerOneTimer
        /// </summary>
        private void resetPlayerOneTimer()
        {
            p_msCount = 0;
            p_secondCount = 0;
            p_minuteCount = 0;
            playerOneStarted = DateTime.UtcNow;
            player_timer_text.Text = "";
        }

        /// <summary>
        /// Resets the PlayerTwoTimer
        /// </summary>
        private void resetPlayerTwoTimer()
        {
            c_msCount = 0;
            c_secondCount = 0;
            c_minuteCount = 0;
            playerTwoStarted = DateTime.UtcNow;
            cpu_timer_text.Text = "";

        }

        /// <summary>
        /// Sets the DataContext of the Rectangles to the corresponding squares
        /// </summary>
        private void bindRectangles()
        {

            int row = board.GetSize()-2;
            int col = 1;
            int i = 0;

            // Loops through each square on the board and binds each rectangle to its corresponding square
            for (col = 1; col <= board.GetSize() - 2; ++col)
            {
                for (row = board.GetSize() - 2; row > 0; --row)
                {
                    rectArr[i].DataContext = board.GetSquare(row, col);
                    ++i;
                }
            }
        }

        #region display methods

        /// <summary>
        /// Refreshes the display of the board, displaying the appropriate image for each square
        /// </summary>
        /// <param name="rectArr">The array containing all rectangles</param>
        /// <param name="board">The current state of the <see cref="Board"/></param>
        private void displayPieces(Rectangle[] rectArr, Board board)
        {
            int row = board.GetSize() - 2;
            int col = 1;
            int i = 0;

            blackImageBrush.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/PieceBlack.png"));
            whiteImageBrush.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/PieceWhite.png"));
            kingImageBrush.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/PieceKing.png"));

            // Loops through each square on the board and binds each rectangle to its corresponding square
            for (col = 1; col <= board.GetSize() - 2; ++col)
            {
                for (row = board.GetSize() - 2; row > 0; --row)
                {
                    Square square = board.GetSquare(row, col);

                    // If there's a piece on the square at the row and column
                    if (square.getPiece() != null)
                    {
                        Piece piece = board.GetSquare(row, col).getPiece();

                        if (piece.getType().Equals(Enums.PieceType.PAWN))
                        {
                            if (piece.getColor().Equals(Enums.Color.BLACK))
                            {
                                if (rectArr[i].Fill != blackImageBrush)
                                    rectArr[i].Fill = blackImageBrush;
                            }
                            else if (piece.getColor().Equals(Enums.Color.WHITE))
                            {
                                if (rectArr[i].Fill != whiteImageBrush)
                                    rectArr[i].Fill = whiteImageBrush;
                            }
                        }
                        // If the piece is a King
                        else if (piece.getType().Equals(Enums.PieceType.KING))
                        {
                            if (rectArr[i].Fill != kingImageBrush)
                                rectArr[i].Fill = kingImageBrush;
                        }
                    }
                    // If there's no piece on the square
                    else if (square.getPiece() == null)
                    {
                        rectArr[i].Fill = null;

                    }
                    rectArr[i].Stroke = null;
                    rectArr[i].RadiusX = 0;
                    rectArr[i].RadiusY = 0;
                    rectArr[i].Opacity = 1;
                    ++i;
                }
            }
        }

        /// <summary>
        /// Applies a stroke to the current player's pieces to highlight them.
        /// </summary>
        private void highlightPieces()
        {
            Brush highlightStrokeBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA2, 0x00));
            double strokeThickness = 2;

            int row = board.GetSize() - 2;
            int col = 1;
            int i = 0;

            for (col = 1; col <= board.GetSize() - 2; ++col)
            {
                for (row = board.GetSize() - 2; row > 0; --row)
                {
                    Piece piece = board.GetSquare(row, col).getPiece();
                    Player currentPlayer = game.getCurrentPlayer();

                    if (piece != null && currentPlayer.getColor().Equals(piece.getColor()))
                    {
                        rectArr[i].Stroke = highlightStrokeBrush;
                        rectArr[i].StrokeThickness = strokeThickness;
                        rectArr[i].RadiusX = 100;
                        rectArr[i].RadiusY = 100;
                    }
                    ++i;
                }
            }
        }

        /// <summary>
        /// Displays to the current player that it is their turn to move.
        /// </summary>
        private void notifyPlayer()
        {
            // If the current player is the attacker (Player 1)
            if (game.getCurrentPlayer().getColor() == Enums.Color.BLACK)
            {
                // Set the active colours for Player 1
                player1Rectangle.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA2, 0x00));
                p1TurnIndicator.Text = "Your Turn";

                // Reset the colors for Player 2
                player2Rectangle.Stroke = null;
                p2TurnIndicator.Text = "";

                highlightPieces();
            }
            // Otherwise, the current player is the defender (Player 2)
            else
            {
                // Set the active colors for Player 2
                player2Rectangle.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA2, 0x00));
                p2TurnIndicator.Text = "Your Turn";

                // Reset the colors for Player 1
                player1Rectangle.Stroke = null;
                p1TurnIndicator.Text = "";

                highlightPieces();
            }
        }

        /// <summary>
        /// Displays the squares that a piece can move to
        /// </summary>
        /// <param name="rect">The tapped rectangle</param>
        private void displayLegalMoves(Rectangle rect)
        {
            Square originSquare = (Square)rect.DataContext;
            Piece piece = originSquare.getPiece();
            Player currentPlayer = game.getCurrentPlayer();

            // If there's a piece on the the tapped rectangle and it matches the color of the current player
            if (piece != null && currentPlayer.getColor().Equals(piece.getColor()))
            {
                // Generate the legal moves for that piece
                List<Square> legalMoves = piece.generateLegalMoves(board);

                int row = board.GetSize() - 2;
                int col = 1;
                int i = 0;

                // Loop through each square on the board and color the squares the piece can move to
                for (col = 1; col <= board.GetSize() - 2; ++col)
                {
                    for (row = board.GetSize() - 2; row > 0; --row)
                    {
                        foreach (var move in legalMoves)
                        {
                            if (board.GetSquare(row, col) == move)
                            {
                                rectArr[i].Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x57, 0x13, 0x01));
                                rectArr[i].RadiusX = 100;
                                rectArr[i].RadiusY = 100;
                                rectArr[i].StrokeThickness = 2;
                                rectArr[i].Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA2, 0x00));
                                rectArr[i].Opacity = 0.6;
                            }
                        }
                        ++i;
                    }
                }
            }
        }

        /// <summary>
        /// Displays the list of moves taken by each player
        /// </summary>
        private void displayMoveNotation()
        {
            p1MoveListText.Text = "History\n";
            p2MoveListText.Text = "History\n";

            p1MoveList = game.getPlayer1().getMoveList();
            p2MoveList = game.getPlayer2().getMoveList();

            foreach (var move in p1MoveList)
            {
                p1MoveListText.Text += move;
            }
            foreach (var move in p2MoveList)
            {
                p2MoveListText.Text += move;
            }
        }

        #endregion

        #region input methods

        /// <summary>
        /// Ends the game with Player 2 as the winner.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void resignBtn1_tapped(object sender, RoutedEventArgs e)
        {
            playerOneTimer.Stop();
            playerTwoTimer.Stop();
            bw.CancelAsync();
            // If the current player is player 1
            if (game.getCurrentPlayer().getColor() == Enums.Color.BLACK)
            {
                game.PlaySound("btn_click.wav");
                game.getPlayer2().setWin(true);
                winText.Text = "Player 2 Wins";
                game.PlaySound("victory_fanfare.wav");
                await Task.Delay(TimeSpan.FromSeconds(5));
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Returns the user to the previous page
        /// </summary>
        /// <param name="sender"><see cref="back_btn"/></param>
        /// <param name="e"></param>
        private void back_btn_clicked(object sender, RoutedEventArgs e)
        {
            bw.CancelAsync();
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        /// <summary>
        /// Enables and disables fullscreen mode
        /// </summary>
        /// <param name="sender"><see cref="fullscreen_btn"/></param>
        /// <param name="e"></param>
        private void fullscreen_btn_clicked(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            ImageBrush fullscreenImg = new ImageBrush();

            bool isInFullScreenMode = view.IsFullScreenMode;

            if (isInFullScreenMode)
            {
                fullscreenImg.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/fullscreen-arrows.png"));
                view.ExitFullScreenMode();
            }
            else
            {
                fullscreenImg.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/windowed-arrows.png"));
                view.TryEnterFullScreenMode();
            }
            this.fullscreen_btn.Background = fullscreenImg;
        }

        /// <summary>
        /// Ends the game with Player 1 as the winner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void resignBtn2_tapped(object sender, RoutedEventArgs e)
        {
            playerOneTimer.Stop();
            playerTwoTimer.Stop();
            bw.CancelAsync();
            // If the current player is player 2
            if (game.getCurrentPlayer().getColor().Equals(Enums.Color.WHITE))
            {
                game.PlaySound("btn_click.wav");
                game.getPlayer1().setWin(true);
                winText.Text = "Player 1 Wins";
                game.PlaySound("victory_fanfare.wav");
                await Task.Delay(TimeSpan.FromSeconds(5));
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Undoes the last move taken
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoBtn_tapped(object sender, RoutedEventArgs e)
        {
            if (game.getCurrentPlayer().getColor().Equals(Enums.Color.BLACK))
            {
                playerOneTimer.Stop();
                resetPlayerTwoTimer();
                playerTwoTimer.Start();

            }
            else
            {
                playerTwoTimer.Stop();
                resetPlayerOneTimer();
                playerOneTimer.Start();
            }
            bw.CancelAsync();
            game.PlaySound("btn_click.wav");
            board = game.undoMove();
            bindRectangles();
            displayPieces(rectArr, board);
            displayMoveNotation();
            notifyPlayer();
            lastTappedRect = null;
        }

        /// <summary>
        /// Performs an action based on the rectangle that was tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void rectangle_tapped(object sender, RoutedEventArgs e)
        {
            displayPieces(rectArr, board);
            Rectangle rect = sender as Rectangle;
            Square tappedSquare = (Square)rect.DataContext;
            Piece piece = tappedSquare.getPiece();
            Player currentPlayer = game.getCurrentPlayer();


            if (currentPlayer.getType().Equals(Enums.PlayerType.HUMAN) && !game.checkWin())
            {
                // If a rectangle was previously tapped
                if (rect != lastTappedRect && lastTappedRect != null)
                {
                    // and the rectangle's bound square contains a piece of the same color as the current player
                    if (piece != null && currentPlayer.getColor().Equals(piece.getColor()))
                    {
                        displayPieces(rectArr, board);
                        // Reset the stroke of the previously tapped square
                        lastTappedRect.Stroke = stationaryStrokeBrush;
                        lastTappedRect.StrokeThickness = strokeThickness;
                        stationaryStrokeBrush = rect.Stroke;
                        strokeThickness = rect.StrokeThickness;

                        // Set the stroke of the currently tapped square
                        rect.StrokeThickness = transformingStrokeThickness;
                        rect.Stroke = transformingStrokeBrush;
                        // Display any legal moves for the currently selected piece
                        displayLegalMoves(rect);
                        lastTappedRect = rect;

                        // Play a sound notifying the player that a piece has been selected.
                        game.PlaySound("piece_select.wav");
                    }
                    // If the square is not a corner or throne square and there is no piece on the tapped square
                    else if (piece == null && !(tappedSquare.getSquareType().Equals(Enums.SquareType.CORNER) || tappedSquare.getSquareType().Equals(Enums.SquareType.THRONE)))
                    {
                        // Set the square containing the selected piece equal to the last tapped square
                        Square originSquare = (Square)lastTappedRect.DataContext;
                        // Set the selected piece equal to the piece on the selected square
                        Piece selectedPiece = originSquare.getPiece();

                        if (selectedPiece != null)
                        {
                            // Make Human Move
                            processMove(tappedSquare, originSquare, selectedPiece);
                            await Task.Delay(TimeSpan.FromSeconds(2));
                            currentPlayer = game.getCurrentPlayer();

                            if (bw.IsBusy != true)
                            {
                                mctsBoard = new Board(board);
                                bw.RunWorkerAsync();
                            }

                        }
                    }
                }
                // If no rectangle was previously tapped
                else
                {
                    // If the tapped square contains a piece and it is of the same color as the current player.
                    if (piece != null && currentPlayer.getColor().Equals(piece.getColor()))
                    {
                        // Backup the default stroke for the square.
                        stationaryStrokeBrush = rect.Stroke;
                        strokeThickness = rect.StrokeThickness;

                        // Set the stroke of the square
                        rect.StrokeThickness = transformingStrokeThickness;
                        rect.Stroke = transformingStrokeBrush;
                        // Display the legal moves for the piece on that square
                        displayLegalMoves(rect);
                        // Set this square as the last tapped rectangle
                        lastTappedRect = rect;
                        // Play a sound indicating that a piece has been selected.
                        game.PlaySound("piece_select.wav");
                    }
                }
            }
        }

        /// <summary>
        /// Processes a move for the CPU player
        /// </summary>
        /// <param name="currentPlayer">The current <see cref="Player"/> of the <see cref="Game"/></param>
        /// <param name="cpuMovesTuple">A <see cref="Tuple{T1, T2, T3}"/> containing the origin <see cref="Square"/>, 
        /// destination <see cref="Square"/> and selected <see cref="Piece"/> to move.</param>
        private async void moveCPU(Player currentPlayer, Tuple<Square, Piece, Square> cpuMovesTuple)
        {
            Square origin = getRectSquare(cpuMovesTuple.Item1);
            Piece selectedCPUPiece = origin.getPiece();
            Square destination = getRectSquare(cpuMovesTuple.Item3);

            Rectangle rect = getCPURect(origin);
            // Set the stroke of the currently tapped square
            rect.StrokeThickness = transformingStrokeThickness;
            rect.Stroke = transformingStrokeBrush;
            // Play a sound notifying the player that a piece has been selected.
            game.PlaySound("piece_select.wav");
            // Display any legal moves for the currently selected piece
            displayLegalMoves(rect);
            await Task.Delay(TimeSpan.FromSeconds(2));
            processMove(destination, origin, selectedCPUPiece);
        }

        /// <summary>
        /// Gets the <see cref="Rectangle"/> Square that matches the <see cref="Square"/> passed in.
        /// </summary>
        /// <param name="square">The <see cref="Square"/> to match against.</param>
        /// <returns></returns>
        private Square getRectSquare(Square square)
        {
            for (int i = 0; i < rectArr.Length; ++i)
            {
                Square rectSquare = rectArr[i].DataContext as Square;
                if (rectSquare.getFile().Equals(square.getFile()) && rectSquare.getRank().Equals(square.getRank()))
                {
                    return rectSquare;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks the <see cref="Game"/> to see if it has been one and displays the status to the <see cref="Player"/>
        /// </summary>
        private async void checkWinAndDisplay()
        {
            // Check for a win
            if (game.checkWin() || checkMate)
            {
                // And display the appropriate win message if a player has won.
                if (game.getPlayer1().getWin())
                {
                    winText.Text = game.getPlayer1().getName() + " Wins!";
                }
                else if (game.getPlayer2().getWin())
                {
                    winText.Text = game.getPlayer2().getName() + " Wins!";
                }
                // Play a jingle to signal a win.
                game.PlaySound("victory_fanfare.wav");
                // Wait 5 seconds to allow the jingle to play.
                await Task.Delay(TimeSpan.FromSeconds(5));
                // Return to the previous screen.
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Checks to see if the King can reach one or more board edges on the next turn
        /// </summary>
        /// <param name="piece">The <see cref="Piece"/> passed into check against.</param>
        private async void canEscape(Piece piece)
        {
            int edgeCount = 0;
            List<Square> kingsMoves = piece.generateLegalMoves(board);
            for (int i = 0; i < kingsMoves.Count; ++i)
            {
                Square currSq = (Square)kingsMoves[i];
                if (currSq.getFile().Equals(Enums.File.A) || currSq.getFile().Equals(Enums.File.K) || currSq.getRank().Equals(Enums.Rank.One) || currSq.getRank().Equals(Enums.Rank.Eleven))
                {
                    ++edgeCount;
                }
            }
            if (edgeCount == 1)
            {
                winText.Text = "Check!";
                await Task.Delay(TimeSpan.FromSeconds(2));
                winText.Text = "";
            }
            else if (edgeCount > 1)
            {
                winText.Text = "Checkmate!";
                await Task.Delay(TimeSpan.FromSeconds(2));
                winText.Text = "";
                checkMate = true;
                checkWinAndDisplay();
            }
        }

        /// <summary>
        /// Returns the <see cref="Rectangle"/> that matches the <see cref="Square"/> passed in
        /// </summary>
        /// <param name="square">The square to compare against.</param>
        /// <returns></returns>
        private Rectangle getCPURect(Square square)
        {
            for (int i = 0; i < rectArr.Length; ++i)
            {
                if (rectArr[i].DataContext.Equals(square))
                {
                    return rectArr[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Processes the moves made by a player
        /// </summary>
        /// <param name="destination">The desintation <see cref="Square"/> to move the <see cref="Piece"/> to.</param>
        /// <param name="origin">The origin <see cref="Square"/> the <see cref="Piece"/> is moving from.</param>
        /// <param name="piece">The <see cref="Piece"/> being moved.</param>
        private void processMove(Square destination, Square origin, Piece piece)
        {
            displayPieces(rectArr, board);
            // Move the piece to the currently tapped square
            game.movePiece(destination, origin, piece);
            // Check to see if the king can escape
            if (piece.getType().Equals(Enums.PieceType.KING))
            {
                canEscape(piece);
            }
            // Update the display of the pieces on the board
            displayPieces(rectArr, board);
            // Update the display of the move notation.
            displayMoveNotation();
            // Check for a win
            checkWinAndDisplay();
            // Notify the next player that the turn is completed.
            notifyPlayer();
            if (game.getCurrentPlayer().getColor().Equals(Enums.Color.WHITE))
            {
                playerOneTimer.Stop();
                resetPlayerTwoTimer();
                playerTwoTimer.Start();
            }
            else if (game.getCurrentPlayer().getColor().Equals(Enums.Color.BLACK))
            {
                playerTwoTimer.Stop();
                resetPlayerOneTimer();
                playerOneTimer.Start();
            }
        }
    }
    //////////////////
    /// End Class ///
    /////////////////
    #endregion




}