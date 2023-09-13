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
        }

        private async void myButton_Click(object sender, RoutedEventArgs e)
        {
            var client = new LemmyHttp("https://lemmy.world");

            GetCommunity getCommunity = new GetCommunity
            {
                Name = "pics"
            };
            var comRes = await client.GetCommunity(getCommunity);
            var comView = comRes.CommunityView;
            var com = comView.Community;
            Log(com.Description);

            GetPosts getPosts = new GetPosts
            {
                CommunityId = com.Id,
                Limit = 40,
            };
            var getPostsRes = await client.GetPosts(getPosts);
            foreach(var postView in getPostsRes.Posts)
            {
                var post = postView.Post;
                Log(post.Name);
                if(IsImageUrl(post.Url))
                {
                    //AddImage(post.Url);
                }
                else
                {
                    Log("Not an image: " + post.Url);
                }
            }

            var imagePosts = getPostsRes.Posts.Select(postView => postView.Post).Where(post => IsImageUrl(post.Url)).ToList();
            repeater.ItemsSource = imagePosts;
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
            Image image = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.UriSource = new Uri(url);
            image.Source = bitmap;

            sp1.Children.Add(image);
        }
    }
}
