using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using VikingChess.Model;
using Windows.UI;

namespace VikingChess
{
    /// <summary>
    /// The main page of the Viking Chess Application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Game game;

        /// <summary>
        /// The MainPage Constructor
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            game = new Game();
        }

        /// <summary>
        /// Processes the button click for the newGameBtn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.Frame.Navigate(typeof(SettingsPage));
        }

        /// <summary>
        /// Processes the button click for the howToBtn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void howToBtn_Click(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.Frame.Navigate(typeof(RulesPage));
        }

        /// <summary>
        /// Processes the button click for the howToBtn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void aboutBtn_Click(object sender, RoutedEventArgs e)
        {
            game.PlaySound("btn_click.wav");
            await Task.Delay(TimeSpan.FromSeconds(1));
            this.Frame.Navigate(typeof(AboutPage));
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

    }
}
