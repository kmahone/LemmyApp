using Lemmy.Net.Types;
using Lemmy.Net;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LemmyApp1
{
    public sealed partial class CommunityPicker : UserControl
    {
        public CommunityPicker()
        {
            this.InitializeComponent();
            asb.TextChanged += Asb_TextChanged;   
        }

        public string Result
        {
            get
            {
                return asb.Text;
            }
        }

        private async void Asb_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var query = asb.Text;
            Search search = new Search
            {
                Type = SearchType.Communities,
                Query = query,
                ListingType = ListingType.All,
                Sort = SortType.TopAll,
                Limit = 20,
            };
            var res = await client.Search(search);
            foreach (var com in res.Communities)
            {
                Debug.WriteLine(com.Community.Name);
            }

            var items = res.Communities.Select(c => c.Community).Select(c => new CommunityInfo(c)).ToList();

            asb.ItemsSource = items;
        }

        LemmyHttp client = new LemmyHttp("https://lemmy.world");
    }
}
