using Lemmy.Net;
using Lemmy.Net.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace LemmyApp1
{
    class LemmyAppVM : INotifyPropertyChanged
    {
        public LemmyAppVM()
        {
            Client = new LemmyHttp("https://lemmy.world");
        }

        public LemmyHttp Client { get; private set; }

        private ObservableCollection<Community> _communities;
        public ObservableCollection<Community> Communities 
        {
            get { return _communities; }
            private set
            {
                _communities = value;
                NotifyPropertyChanged(nameof(Communities));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public LemmyPostsVM GetVMForCommunity(Community community)
        {
            return new LemmyPostsVM(this, community);
        }

        public async void Setup()
        {
            var jsonString = File.ReadAllText(@"C:\FHL\LemmyApp\lemmyconfig.json");
            UserCredentials userCredentials = JsonSerializer.Deserialize<UserCredentials>(jsonString);

            var success = await Client.Login(userCredentials.Username, userCredentials.Password);
            if (!success) { throw new InvalidOperationException("login failed"); }

            ListCommunities lc = new ListCommunities
            {
                Type = ListingType.Subscribed,
            };

            var comms = Client.ListAllCommunities(lc);

            ObservableCollection<Community> communities = new ObservableCollection<Community>();
            await foreach (var cv in comms)
            {
                Debug.WriteLine(cv.Community.Name);
                communities.Add(cv.Community);
            }

            Communities = communities;
        }

        public class UserCredentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
