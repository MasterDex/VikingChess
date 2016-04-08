using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace VikingChess
{
    /// <summary>
    /// The AboutPage details information about the development of this application
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        /// <summary>
        /// Constructor for AboutPage
        /// </summary>
        public AboutPage()
        {
            this.InitializeComponent();
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
