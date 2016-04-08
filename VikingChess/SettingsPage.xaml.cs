using System;
using VikingChess.Model;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VikingChess
{
    /// <summary>
    /// The SettingsPage allows for the setting of game parameters such as playing locally against a human or against the computer, setting player names and choosing the game type.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        Game game = new Game();

        /// <summary>
        /// SettingsPage Constructor
        /// </summary>
        public SettingsPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
        }

        /// <summary>
        /// Processes the button click for the backBtn
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
        /// Processes the button click for the fullscreenBtn
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
        /// Gets the parameters for the <see cref="Game"/>
        /// </summary>
        /// <returns><see cref="Tuple{T1, T2, T3, T4, T5}"/></returns>
        private Tuple<string, string, string, string> getParameters()
        {
            string versusChoice;
            string sideChoice;
            string p1Name;
            string p2Name;

            if (vsToggleSwitch.IsOn)
            {
                versusChoice = "CPU";
            }
            else
            {
                versusChoice = "HUMAN";
            }
            if (colorSwitch.IsOn)
            {
                sideChoice = "DEFENDER";
            }
            else
            {
                sideChoice = "ATTACKER";
            }

            p1Name = p1NameTextBox.Text;
            p2Name = p2NameTextBox.Text;

            return new Tuple<string, string, string, string>(versusChoice, sideChoice, p1Name, p2Name);
        }

        /// <summary>
        /// Starts a game of Hnefatafl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HnefataflBtn_Clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");

            Tuple<string, string, string, string> parameters = getParameters();

            Tuple<string, string, string, string, string> gameSettings = new Tuple<string, string, string, string, string>(parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4, "Hnefatafl");

            Frame.Navigate(typeof(HnefataflPage), gameSettings);
        }

        /// <summary>
        /// Starts a game of Brandubh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrandubhBtn_Clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");

            Tuple<string, string, string, string> parameters = getParameters();

            Tuple<string, string, string, string, string> gameSettings = new Tuple<string, string, string, string, string>(parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4, "Brandubh");

            Frame.Navigate(typeof(BrandubhPage), gameSettings);
        }

        /// <summary>
        /// Starts a game of Ard Rí
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArdRiBtn_Clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");

            Tuple<string, string, string, string> parameters = getParameters();

            Tuple<string, string, string, string, string> gameSettings = new Tuple<string, string, string, string, string>(parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4, "Ard Ri");

            Frame.Navigate(typeof(ArdRiPage), gameSettings);
        }

        /// <summary>
        /// Starts a game of Tablut
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TablutBtn_Clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");

            Tuple<string, string, string, string> parameters = getParameters();

            Tuple<string, string, string, string, string> gameSettings = new Tuple<string, string, string, string, string>(parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4, "Tablut");

            Frame.Navigate(typeof(TablutPage), gameSettings);
        }

        /// <summary>
        /// Starts a game of Tawlbwrdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TawlbwrddBtn_Clicked(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");

            Tuple<string, string, string, string> parameters = getParameters();

            Tuple<string, string, string, string, string> gameSettings = new Tuple<string, string, string, string, string>(parameters.Item1, parameters.Item2, parameters.Item3, parameters.Item4, "Tawlbwrdd");

            Frame.Navigate(typeof(TawlbwrddPage), gameSettings);
        }
    }
}
