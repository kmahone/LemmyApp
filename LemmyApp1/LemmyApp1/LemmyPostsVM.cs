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

        ObservableCollection<PostView> posts;
        public ObservableCollection<PostView> Posts 
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
        int pageSize = 10;
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

            communityId = com.Id;

            GetPosts getPosts = new GetPosts
            {
                CommunityId = communityId,
                Limit = pageSize,
                Page = 1,
            };
            var getPostsRes = await client.GetPosts(getPosts);
            var imagePosts = getPostsRes.Posts.Where(postView => IsImageUrl(postView.Post.Url));
            Posts = new ObservableCollection<PostView>(imagePosts);

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

            page++;
            GetPosts getPosts = new GetPosts
            {
                CommunityId = communityId,
                Limit = pageSize,
                Page = page,
            };
            var getPostsRes = await client.GetPosts(getPosts);

            var imagePosts = getPostsRes.Posts.Where(postView => IsImageUrl(postView.Post.Url));
            foreach (var post in imagePosts)
            {
                Posts.Add(post);
            }
            isLoadingMoreItems = false;
        }

        private bool IsImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".gif") || url.EndsWith(".jpeg");
        }
    }
}
