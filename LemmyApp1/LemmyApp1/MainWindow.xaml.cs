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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LemmyApp1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            scrollViewer1.ViewChanged += ScrollViewer1_ViewChanged;
        }

        bool setupRan = false;
        bool isLoadingMoreItems = false;

        ObservableCollection<Post> imagePosts;
        LemmyHttp client;
        long communityId;
        int page = 1;

        async void LoadMoreItems()
        {
            if(isLoadingMoreItems)
            {
                return;
            }
            isLoadingMoreItems = true;
            Log("LoadMoreItems!");
            page++;
            GetPosts getPosts = new GetPosts
            {
                CommunityId = communityId,
                Limit = 10,
                Page = page,
            };
            var getPostsRes = await client.GetPosts(getPosts);

            var imagePosts1 = getPostsRes.Posts.Select(postView => postView.Post).Where(post => IsImageUrl(post.Url));
            foreach (var post in imagePosts1) 
            {
                imagePosts.Add(post);
            }
            isLoadingMoreItems = false;
        }


        private async Task Setup()
        {
            client = new LemmyHttp("https://lemmy.world");

            GetCommunity getCommunity = new GetCommunity
            {
                Name = "pics"
            };
            var comRes = await client.GetCommunity(getCommunity);
            var comView = comRes.CommunityView;
            var com = comView.Community;
            //Log(com.Description);

            communityId = com.Id;

            GetPosts getPosts = new GetPosts
            {
                CommunityId = communityId,
                Limit = 10,
                Page = 1,
            };
            var getPostsRes = await client.GetPosts(getPosts);
            foreach (var postView in getPostsRes.Posts)
            {
                var post = postView.Post;
                //Log(post.Name);
                if (IsImageUrl(post.Url))
                {
                    //AddImage(post.Url);
                }
                else
                {
                    Log("Not an image: " + post.Url);
                }
            }

            var imagePosts1 = getPostsRes.Posts.Select(postView => postView.Post).Where(post => IsImageUrl(post.Url));
            imagePosts = new ObservableCollection<Post>(imagePosts1);
            repeater.ItemsSource = imagePosts;

            setupRan = true;
        }


        private void ScrollViewer1_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if(setupRan)
            {
                var offset = scrollViewer1.VerticalOffset;
                var extent = scrollViewer1.ExtentHeight;
                //Log($"ScrollViewer1_ViewChanged offset={offset}, extent={extent}, IsIntermediate={e.IsIntermediate} ");

                var viewport = scrollViewer1.ViewportHeight;
                var boundary = extent - (3 * viewport);
                if (offset > boundary)
                {
                    LoadMoreItems();
                }
            }
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Setup().GetAwaiter();
        }

        private bool IsImageUrl(string url)
        {
            if(string.IsNullOrEmpty(url)) return false; 

            return url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".gif") || url.EndsWith(".jpeg");
        }

        private void Log(string message) 
        {
            tb1.Text += message + Environment.NewLine;
        }

        private void AddImage(string url)
        {
            //Image image = new Image();
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.UriSource = new Uri(url);
            //image.Source = bitmap;

            //sp1.Children.Add(image);
        }
    }
}
