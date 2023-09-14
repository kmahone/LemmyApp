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
    public sealed partial class LemmyPostsViewerPage : Page
    {
        public LemmyPostsViewerPage()
        {
            this.InitializeComponent();
            scrollViewer1.ViewChanged += ScrollViewer1_ViewChanged;
            this.LayoutRoot.DataContext = vm;

            
            ConfigureWebViewCommentsViewer();
        }

        async void ConfigureWebViewCommentsViewer()
        {
            var js2 = @"document.addEventListener(""DOMContentLoaded"", function() {
                            var root = document.getElementById(""root"");
                            var comments = document.getElementsByClassName(""comments"");
                            var newContent = ""No Comments"";
                            if(comments.length > 0)
                            {
                              newContent = comments[0];
                            }
                            root.replaceWith(newContent);
            });";
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js2);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var item = e.Parameter as string;
            vm.Setup(item);
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

        private async void commentsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnItemComments_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var parent = button.Parent as UIElement;

            var index = repeater.GetElementIndex(parent);
            var postView = repeater.ItemsSourceView.GetAt(index) as PostView;

            
            
            webView.Source = new Uri($"https://lemmy.ml/post/{postView.Post.Id}");

        }

        private void gcBtn_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}