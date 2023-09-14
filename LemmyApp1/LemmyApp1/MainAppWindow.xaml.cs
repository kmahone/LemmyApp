using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LemmyApp1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainAppWindow : Window
    {
        public MainAppWindow()
        {
            this.InitializeComponent();
            frame1.Navigate(typeof(LemmyPostsViewerPage));
            navigationView.MenuItemsSource = new[]
            {
                "memes",
                "pics",
                "programmerhumor",
                "badrealestate@feddit.uk",
                "cats@lemmy.world"
            };

            navigationView.SelectionChanged += NavigationView_SelectionChanged;
            navigationView.BackRequested += NavigationView_BackRequested;
            navigationView.SelectedItem = "memes";
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(frame1.CanGoBack)
            {
                frame1.GoBack();
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = navigationView.SelectedItem as string;
            navigationView.Header = item;
            frame1.Navigate(typeof(LemmyPostsViewerPage), item);
        }
    }
}
