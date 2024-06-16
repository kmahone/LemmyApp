using Lemmy.Net.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemmyApp1
{
    internal class LemmyCommunitiesVM
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        ObservableCollection<string> _communities = new ObservableCollection<string>
        {
            "memes",
            "pics",
            "programmerhumor",
            "badrealestate@feddit.uk",
            "cat@lemmy.world"
        };

        public ObservableCollection<string> Communities
        {
            get
            {
                return _communities;
            }
            private set
            {
                _communities = value;
                NotifyPropertyChanged(nameof(Communities));
            }
        }
    }

    public class CommunityInfo
    {
        public CommunityInfo(string name, long id)
        {
            Name = name;
            Id = id;
        }

        public CommunityInfo(Community community)
        {
            Name = community.Name;
            Id = community.Id;
            Host = new Uri(community.ActorId).Host;
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public string Host { get; set; }

        public override string ToString()
        {
            if(string.IsNullOrEmpty(Host) || Host == "lemmy.world")
            {
                return Name;
            }
            else 
            {
                return $"{Name}@{Host}";
            }
        }
    }
}
