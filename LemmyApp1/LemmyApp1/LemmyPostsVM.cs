using Lemmy.Net.Types;
using Lemmy.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemmyApp1
{
    internal class LemmyPostsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) 
            { 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ObservableCollection<Post> posts;
        public ObservableCollection<Post> Posts 
        { 
            get
            {
                return posts;
            }
            private set
            {
                posts = value;
                NotifyPropertyChanged(nameof(Posts));
            }
        }

        bool setupRan = false;
        public bool SetupRan
        {
            get 
            { 
                return setupRan; 
            } 
            set 
            { 
                setupRan = value;
                NotifyPropertyChanged(nameof(SetupRan));
            }
        }

        bool isLoadingMoreItems = false;

        LemmyHttp client;
        long communityId;
        int page = 1;

        public async void Setup()
        {
            client = new LemmyHttp("https://lemmy.ml");

            GetCommunity getCommunity = new GetCommunity
            {
                Name = "memes"
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
                    //Log("Not an image: " + post.Url);
                }
            }

            var imagePosts1 = getPostsRes.Posts.Select(postView => postView.Post).Where(post => IsImageUrl(post.Url));
            Posts = new ObservableCollection<Post>(imagePosts1);

            SetupRan = true;
        }

        public async void LoadMoreItems()
        {
            if(!SetupRan)
            {
                return;
            }
            if (isLoadingMoreItems)
            {
                return;
            }
            isLoadingMoreItems = true;
            //Log("LoadMoreItems!");
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
                Posts.Add(post);
            }
            isLoadingMoreItems = false;
        }

        private bool IsImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            if (url.Contains("lemmy.world")) return false;

            return url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".gif") || url.EndsWith(".jpeg");
        }
    }
}
