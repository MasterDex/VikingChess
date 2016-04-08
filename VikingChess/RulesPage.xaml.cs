using System;
using VikingChess.Model;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using XamlAnimatedGif;


namespace VikingChess
{
    /// <summary>
    /// The RulesPage displays information on how to play the game.
    /// </summary>
    public sealed partial class RulesPage : Page
    {
        Game game;

        /// <summary>
        /// Constructor for RulesPage
        /// </summary>
        public RulesPage()
        {
            this.InitializeComponent();
            game = new Game();

        }

        /// <summary>
        /// Returns the user to the main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void back_btn_clicked(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        /// <summary>
        /// Puts the application into fullscreen
        /// </summary>
        /// <param name="sender"></param>
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
                this.fullscreen_btn.Background = fullscreenImg;
            }
            else
            {
                fullscreenImg.ImageSource = new BitmapImage(new Uri(this.BaseUri, "/Assets/windowed-arrows.png"));
                view.TryEnterFullScreenMode();
                this.fullscreen_btn.Background = fullscreenImg;
            }
        }

        /// <summary>
        /// Displays the movement text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void movementBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/Moving.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/MovementText.gif"));
        }

        /// <summary>
        /// Displays the capture text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/Capturing.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/CapturingText.gif"));
        }

        /// <summary>
        /// Displays the attacker's win condition text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winAttackBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/WinAttackers.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/WinAttackerText.gif"));
        }

        /// <summary>
        /// Displays the defender's win condition text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winDefBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/WinDefenders.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/WinDefenderText.gif"));
        }

        /// <summary>
        /// Displays the Blockade text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void blockBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/StrategyBlockade.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/BlockadeText.gif"));
        }

        /// <summary>
        /// Displays the Tower text and animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void towerBtn_clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            AnimationBehavior.SetSourceUri(gifImg, new Uri(this.BaseUri, "/Assets/StrategyTower.gif"));
            AnimationBehavior.SetSourceUri(textGif, new Uri(this.BaseUri, "/Assets/TowerText.gif"));
        }
    }
}
