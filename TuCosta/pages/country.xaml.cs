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
using System.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;
using TuCosta.classes;
using System.Globalization;
using TuCosta.Resources;

namespace TuCosta.pages
{
    public partial class country : PhoneApplicationPage
    {
        public country()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                string culture = CultureInfo.CurrentCulture.Name.ToString().Substring(0, 2);
                string id = (this.NavigationContext.QueryString["id"]);

                WebClient w = new WebClient();
                Observable
                .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
                .Subscribe(r =>
                {
                    var deserialized = JsonConvert.DeserializeObject<List<types>>(r.EventArgs.Result);
                    txtMapName.Text = deserialized[0].name.ToString();
                });
                w.DownloadStringAsync(
                new Uri("http://tucosta.zz.mu/types.php?idtype=" + id + "&lang=" + culture));

                WebClient w2 = new WebClient();
                Observable
                .FromEvent<DownloadStringCompletedEventArgs>(w2, "DownloadStringCompleted")
                .Subscribe(r =>
                {
                    var deserialized = JsonConvert.DeserializeObject<List<places>>(r.EventArgs.Result);

                    ruinsList.ItemsSource = deserialized;

                    createMap cm = new createMap(map1);

                    cm.setCenter(13.794185, -88.896530, 8, true);

                    List<Tuple<double, double, string, int, int, bool>> locations = new List<Tuple<double, double, string, int, int, bool>>();

                    for (int i = 0; i < deserialized.Count; i++)
                        locations.Add(new Tuple<double, double, string, int, int, bool>(deserialized[i].latitude, deserialized[i].longitude, deserialized[i].place, deserialized[i].idplace, deserialized[i].idtype, false));

                    cm.addPushpins(locations);

                });
                w2.DownloadStringAsync(
                new Uri("http://tucosta.zz.mu/places.php?idtype=" + id + "&lang=" + culture));
            }
            else
                MessageBox.Show(AppResources.Internet, "Error", MessageBoxButton.OK);
        }

        private void PhoneList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBox item = (ListBox)sender;
            if (item.SelectedItem != null)
            {

                places lr = item.SelectedItem as places;

                string url = "/pages/ruinDetail.xaml?id=" + lr.idplace.ToString();
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
            item.SelectedIndex = -1;
        }
    
        private void menuProfile_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/login.xaml", UriKind.Relative));
        }
    }
}