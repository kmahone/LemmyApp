using Lemmy.Net;
using Lemmy.Net.Types;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LemmyApp1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            vm = e.Parameter as LemmyCommunitiesVM;
            

            listview.ItemsSource = vm.Communities;
        }

        LemmyCommunitiesVM vm;

        private void DeleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            var com = (sender as Button).DataContext as string;

            vm.Communities.Remove(com);
        }

        private async void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Add new Community";
            dialog.PrimaryButtonText = "Ok";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;

            CommunityPicker picker = new CommunityPicker();
            dialog.Content = picker;

            var result = await dialog.ShowAsync();

            if(result == ContentDialogResult.Primary)
            {
                vm.Communities.Add(picker.Result);
            }
        }
    }
}
