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
using System.Diagnostics;
using System.Numerics;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI.WebUI;

namespace LemmyApp1
{
    public sealed partial class LemmyPostsViewerPage : Page
    {
        public LemmyPostsViewerPage()
        {
            this.InitializeComponent();
            this.LayoutRoot.DataContext = vm;

            ConfigureWebViewCommentsViewer();
            this.Loaded += LemmyPostsViewerPage_Loaded;

            listview.SelectionChanged += Listview_SelectionChanged;
        }

        private void LemmyPostsViewerPage_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetAllDescendants(listview).OfType<ScrollViewer>().First();
            scrollViewer.ViewChanging += ScrollViewer_ViewChanging;
        }

        public static ImageSource UriStringToImageSource(string uri)
        {
            if(string.IsNullOrEmpty(uri))
            {
                return new BitmapImage();
            }
            else
            {
                return new BitmapImage()
                {
                    UriSource = new Uri(uri),
                };
            }
        }


        private IEnumerable<DependencyObject> GetAllDescendants(DependencyObject element)
        {
            var count = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                yield return child;
                foreach (var descendant in GetAllDescendants(child))
                {
                    yield return descendant;
                }
            }
        }

        async void ConfigureWebViewCommentsViewer()
        {
            var js2 = @"
                document.addEventListener(""DOMContentLoaded"", function() {
                    if(window.location.href.startsWith(""https://lemmy.world/post/""))
                    {
                        var root = document.getElementById(""root"");
                        var comments = document.getElementsByClassName(""comments"");
                        var newContent = ""No Comments"";
                        if(comments.length > 0)
                        {
                            newContent = comments[0];
                        }
                        root.replaceWith(newContent);
                    }
                });";
            await webView.EnsureCoreWebView2Async();

            webView.NavigationStarting += WebView_NavigationStarting;

            var brush = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as SolidColorBrush;
            if(brush != null) 
            {
                webView.DefaultBackgroundColor = brush.Color;
            }

            _ = webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js2);
        }

        private void WebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Debug.WriteLine($"WebView_NavigationStarting: Uri: {args.Uri}, IsUserInitiated: {args.IsUserInitiated}");
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
            webView.Visibility = Visibility.Collapsed;
        }

        private void ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (vm.SetupRan)
            {
                var scrollViewer = sender as ScrollViewer;

                var offset = e.NextView.VerticalOffset;
                var extent = scrollViewer.ExtentHeight;
                var viewport = scrollViewer.ViewportHeight;
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

        private void Listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var postView = listview.SelectedItem as PostView;

            if(postView != null) 
            {
                webView.Source = new Uri($"https://lemmy.world/post/{postView.Post.Id}");
                webView.Visibility = Visibility.Visible;
            }
        }

        private async void btnItemComments_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //var parent = button.Parent as UIElement;

            //var index = repeater.GetElementIndex(parent);
            //var postView = repeater.ItemsSourceView.GetAt(index) as PostView;

            //webView.Source = new Uri($"https://lemmy.world/post/{postView.Post.Id}");
            //webView.Visibility = Visibility.Visible;
        }

        private void gcBtn_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }
    }
}