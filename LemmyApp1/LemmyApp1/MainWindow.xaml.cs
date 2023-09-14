using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Lemmy.Net;
using Lemmy.Net.Types;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Notifications;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace LemmyApp1
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            scrollViewer1.ViewChanged += ScrollViewer1_ViewChanged;
            this.LayoutRoot.DataContext = vm;
            vm.Setup();
            
        }

        LemmyPostsVM vm = new LemmyPostsVM();

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ScrollViewer1_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (vm.SetupRan)
            {
                var offset = scrollViewer1.VerticalOffset;
                var extent = scrollViewer1.ExtentHeight;
                var viewport = scrollViewer1.ViewportHeight;
                var boundary = extent - (3 * viewport);
                if (offset > boundary)
                {
                    vm.LoadMoreItems();
                }
            }
        }

        private void Log(string message) 
        {
            tb1.Text += message + Environment.NewLine;
        }

        private async void commentsBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnItemComments_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var parent = button.Parent as UIElement;

            var index = repeater.GetElementIndex(parent);
            var postView = repeater.ItemsSourceView.GetAt(index) as PostView;

            //var post = button.DataContext as Post;
            webView.Source = new Uri($"https://lemmy.ml/post/{postView.Post.Id}");

            
        }

        private void gcBtn_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}
