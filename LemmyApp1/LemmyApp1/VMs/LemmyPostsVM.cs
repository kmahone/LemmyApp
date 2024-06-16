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
        public LemmyPostsVM(LemmyAppVM appVM, Community community)
        {
            AppVM = appVM;
            Community = community;
        }

        public LemmyAppVM AppVM { get; private set; }

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

        private Community _community;
        public Community Community
        {
            get { return this._community; }
            set
            {
                this._community = value;
                NotifyPropertyChanged(nameof(Community));
            }
        }   
        

        bool isLoadingMoreItems = false;

        long communityId;
        int pageSize = 10;
        int page = 1;

        public async void Setup()
        {
            communityId = Community.Id;

            GetPosts getPosts = new GetPosts
            {
                CommunityId = communityId,
                Limit = pageSize,
                Page = 1,
            };
            
            var getPostsRes = await AppVM.Client.GetPosts(getPosts);
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
            var getPostsRes = await AppVM.Client.GetPosts(getPosts);

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

            return url.EndsWith(".png") || url.EndsWith(".jpg") || url.EndsWith(".gif") || url.EndsWith(".jpeg") || url.EndsWith(".webp");
        }
    }
}
