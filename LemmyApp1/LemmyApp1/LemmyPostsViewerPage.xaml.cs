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
            this.Loaded += LemmyPostsViewerPage_Loaded;

            itemsView.SelectionChanged += ItemsView_SelectionChanged;
            
        }

        private void LemmyPostsViewerPage_Loaded(object sender, RoutedEventArgs e)
        {
            itemsView.ScrollView.ViewChanged += ScrollView_ViewChanged;
            //itemsView.ScrollView.StateChanged += ScrollView_StateChanged;
            //itemsView.ScrollView.IgnoredInputKinds = ScrollingInputKinds.MouseWheel;
        }



        async void ConfigureWebViewCommentsViewer()
        {
            var js2 = @"
                document.addEventListener(""DOMContentLoaded"", function() {
                    if(window.location.href.startsWith(""https://lemmy.ml/post/""))
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

            webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(js2);
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

        private void ScrollView_StateChanged(ScrollView sender, object args)
        {
            if (vm.SetupRan)
            {
                if (sender.State == ScrollingInteractionState.Idle)
                {
                    var offset = sender.VerticalOffset;
                    var extent = sender.ExtentHeight;
                    var viewport = sender.ViewportHeight;
                    var boundary = extent - (3 * viewport);
                    if (offset > boundary)
                    {
                        vm.LoadMoreItems();
                    }
                }
            }
        }

        private void ScrollView_ViewChanged(ScrollView sender, object args)
        {
            if (vm.SetupRan)
            {
                //if(sender.State == ScrollingInteractionState.Idle)
                {
                    var offset = sender.VerticalOffset;
                    var extent = sender.ExtentHeight;
                    var viewport = sender.ViewportHeight;
                    var boundary = extent - (3 * viewport);
                    if (offset > boundary)
                    {
                        vm.LoadMoreItems();
                    }
                }
            }
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

        private void ItemsView_SelectionChanged(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
        {
            var postView = sender.SelectedItem as PostView;

            if(postView != null) 
            {
                webView.Source = new Uri($"https://lemmy.ml/post/{postView.Post.Id}");
                webView.Visibility = Visibility.Visible;
            }
        }

        private async void btnItemComments_Click(object sender, RoutedEventArgs e)
        {
            //var button = sender as Button;
            //var parent = button.Parent as UIElement;

            //var index = repeater.GetElementIndex(parent);
            //var postView = repeater.ItemsSourceView.GetAt(index) as PostView;

            //webView.Source = new Uri($"https://lemmy.ml/post/{postView.Post.Id}");
            //webView.Visibility = Visibility.Visible;
        }

        private void gcBtn_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }

        private void itemsView_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //var delta = e.GetCurrentPoint(itemsView).Properties.MouseWheelDelta;
            //Debug.WriteLine($"itemsView_PointerWheelChanged: delta: {delta}");

            //Vector2 inertiaDecayRate = new Vector2(0.9995f, 0.9995f);
            //Vector2 vector2 = new Vector2(0, -7 * delta);
            //itemsView.ScrollView.AddScrollVelocity(vector2, inertiaDecayRate);
        }
    }
}