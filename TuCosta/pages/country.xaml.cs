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
                new Uri("http://localhost/beach/types.php?idtype=" + id));

                WebClient w2 = new WebClient();
                Observable
                .FromEvent<DownloadStringCompletedEventArgs>(w2, "DownloadStringCompleted")
                .Subscribe(r =>
                {
                    var deserialized = JsonConvert.DeserializeObject<List<places>>(r.EventArgs.Result);

                    ruinsList.ItemsSource = deserialized;

                    createMap cm = new createMap(map1);

                    cm.setCenter(13.794185, -88.896530, 8, true);

                    List<Tuple<double, double, string, int, int>> locations = new List<Tuple<double, double, string, int, int>>();

                    for (int i = 0; i < deserialized.Count; i++)
                        locations.Add(new Tuple<double, double, string, int, int>(deserialized[i].latitude, deserialized[i].longitude, deserialized[i].place, deserialized[i].idplace, deserialized[i].idtype));

                    cm.addPushpins(locations);

                });
                w2.DownloadStringAsync(
                new Uri("http://localhost/beach/places.php?idtype=" + id));
            }
            else
                MessageBox.Show("No hay acceso a internet");
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
    }
}