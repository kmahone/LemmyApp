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
using Lemmy.Net.Types;

namespace LemmyApp1
{
    public sealed partial class MainAppWindow : Window
    {
        public MainAppWindow()
        {
            this.InitializeComponent();

            navigationView.SelectionChanged += NavigationView_SelectionChanged;
            navigationView.BackRequested += NavigationView_BackRequested;

            appVm.Setup();
        }

        public static Uri ImageUrlStringToUri(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return new Uri("http://example.com");
            }
            else
            {
                return new Uri(uri);
            }
        }

        LemmyCommunitiesVM communitiesVM = new LemmyCommunitiesVM();
        LemmyAppVM appVm = new LemmyAppVM();

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if(frame1.CanGoBack)
            {
                frame1.GoBack();
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (navigationView.SelectedItem == navigationView.SettingsItem)
            {
                frame1.Navigate(typeof(SettingsPage), communitiesVM);
            }
            else if(navigationView.SelectedItem is Community community)
            {
                var vm = appVm.GetVMForCommunity(community);
                frame1.Navigate(typeof(LemmyPostsViewerPage), vm);
            }
        }
    }
}
