using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using TuCosta.classes;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.Phone.Reactive;
using System.IO;
using Microsoft.Phone.Tasks;

namespace TuCosta
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {

            WebClient w = new WebClient();

            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
            .Subscribe(r =>
            {
                var deserialized = JsonConvert.DeserializeObject<List<types>>(r.EventArgs.Result);
                lawList.ItemsSource = deserialized;
            });
            w.DownloadStringAsync(
            new Uri("http://localhost/beach/types.php"));

            WebClient w2 = new WebClient();
            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(w2, "DownloadStringCompleted")
            .Subscribe(r =>
            {
                var deserialized = JsonConvert.DeserializeObject<List<places>>(r.EventArgs.Result);

                createMap cm = new createMap(map1);

                cm.setCenter(13.794185, -88.896530, 8, true);

                List<Tuple<double, double, string, int, int, bool>> locations = new List<Tuple<double, double, string, int, int, bool>>();

                for (int i = 0; i < deserialized.Count; i++)
                    locations.Add(new Tuple<double, double, string, int, int, bool>(deserialized[i].latitude, deserialized[i].longitude, deserialized[i].place, deserialized[i].idplace, deserialized[i].idtype, false));

                cm.addPushpins(locations);

            });
            w2.DownloadStringAsync(
            new Uri("http://localhost/beach/places.php?"));


            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("user.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserInfo>));
                        List<UserInfo> data = (List<UserInfo>)serializer.Deserialize(stream);

                    }
                }
            }
            catch
            {

            }

        }

        private void PhoneList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox item = (ListBox)sender;
            if (item.SelectedItem != null)
            {

                types lr = item.SelectedItem as types;

                string url = "/pages/country.xaml?id=" + lr.idtype;
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
            item.SelectedIndex = -1;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/about.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        private void menuProfile_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/login.xaml", UriKind.Relative));
        }

    }
}