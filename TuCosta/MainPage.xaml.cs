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

                List<Tuple<double, double, string, int, int>> locations = new List<Tuple<double, double, string, int, int>>();

                for (int i = 0; i < deserialized.Count; i++)
                    locations.Add(new Tuple<double, double, string, int, int>(deserialized[i].latitude, deserialized[i].longitude, deserialized[i].place, deserialized[i].idplace, deserialized[i].idtype));

                cm.addPushpins(locations);

            });
            w2.DownloadStringAsync(
            new Uri("http://localhost/beach/places.php?"));

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("user.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserInfo>));
                        List<UserInfo> data = (List<UserInfo>)serializer.Deserialize(stream);

                        ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                        menuItem1.Text = "cerrar sesión";
                        ApplicationBar.MenuItems.Add(menuItem1);
                        menuItem1.Click += new EventHandler(Menu1_Click);

                    }
                }
            }
            catch
            {
                ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                menuItem1.Text = "ingresar";
                ApplicationBar.MenuItems.Add(menuItem1);
                menuItem1.Click += new EventHandler(Menu2_Click);
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

        private void Menu2_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/login.xaml", UriKind.Relative));
        }

        private void Menu1_Click(object sender, EventArgs e)
        {
            //Cerrar Sesión
            try
            {
                IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
                storage.DeleteFile("user.xml");
                NavigationService.Navigate(new Uri("/MainPage.xaml?Refresh=true", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void appBar_StateChanged(object sender, Microsoft.Phone.Shell.ApplicationBarStateChangedEventArgs e)
        {
        }
    }
}