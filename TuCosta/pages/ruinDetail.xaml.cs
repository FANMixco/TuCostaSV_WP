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
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using TuCosta.classes;
using System.Net.NetworkInformation;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Phone.Shell;

namespace TuCosta.pages
{
    public partial class ruinDetail : PhoneApplicationPage
    {
        List<UserInfo> data;

        public ruinDetail()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            getComments();

            if (NetworkInterface.GetIsNetworkAvailable())
            {

                int id = int.Parse(this.NavigationContext.QueryString["id"]);

                WebClient w = new WebClient();
                Observable
                .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
                .Subscribe(r =>
                {
                    var deserialized = JsonConvert.DeserializeObject<List<places>>(r.EventArgs.Result);
                    Uri uri = new Uri(deserialized[0].image, UriKind.Absolute);
                    location.Source = new BitmapImage(uri);

                    txtRuinName.Text = deserialized[0].place;
                    detail.Text = deserialized[0].description;

                    createMap cm = new createMap(map1);

                    cm.setCenter(13.794185, -88.896530, 8, true);

                    List<Tuple<double, double, string, int, int>> locations = new List<Tuple<double, double, string, int, int>>();

                    locations.Add(new Tuple<double, double, string, int, int>(deserialized[0].latitude, deserialized[0].longitude, deserialized[0].place, deserialized[0].idplace, deserialized[0].idtype));

                    cm.addPushpins(locations);

                });
                w.DownloadStringAsync(
                new Uri("http://localhost/beach/places.php?idplace=" + id));
            }
            else
                MessageBox.Show("No hay internet");

            ApplicationBar = new ApplicationBar();

            ApplicationBar.Opacity = 0.9;

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Check.png", UriKind.Relative);
            button1.Text = "like";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(ApplicationBarIconButton_Click_1);

            ApplicationBarIconButton button2 = new ApplicationBarIconButton();
            button2.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Cancel.png", UriKind.Relative);
            button2.Text = "dislike";
            ApplicationBar.Buttons.Add(button2);
            button2.Click += new EventHandler(ApplicationBarIconButton_Click);

            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("user.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserInfo>));
                        data = (List<UserInfo>)serializer.Deserialize(stream);

                        ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                        menuItem1.Text = "cerrar sesión";
                        ApplicationBar.MenuItems.Add(menuItem1);
                        menuItem1.Click += new EventHandler(Menu2_Click);

                    }
                }
            }
            catch
            {
                ApplicationBarMenuItem menuItem1 = new ApplicationBarMenuItem();
                menuItem1.Text = "ingresar";
                ApplicationBar.MenuItems.Add(menuItem1);
                menuItem1.Click += new EventHandler(Menu1_Click);
            }

        }

        private void Menu2_Click(object sender, EventArgs e)
        {
            string idtopic = (this.NavigationContext.QueryString["idtopic"]);

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            storage.DeleteFile("user.xml");
            NavigationService.Navigate(new Uri(string.Format("/pages/vote.xaml?idtopic=" + idtopic + "&random={0}" + "&nocache=" + Environment.TickCount + "&Refresh=true", Guid.NewGuid()), UriKind.Relative));
        }

        private void Menu1_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/login.xaml", UriKind.Relative));
        }

        public void saveVote(int vote, string reason)
        {
            string idtopic = (this.NavigationContext.QueryString["id"]);

            WebClient w = new WebClient();

            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
            .Subscribe(r =>
            {
                var deserialized = JsonConvert.DeserializeObject<result>(r.EventArgs.Result);
                if (deserialized.created == 3)
                    MessageBox.Show("¡Ya voto por dicho proyecto, gracias!");
                else if (deserialized.created == 2)
                    MessageBox.Show("¡Lo sentimos intente otro momento!");
                else if (deserialized.created == 0)
                    MessageBox.Show("Necesita ingresar al sistema para votar, ¡Gracias!");
                else
                {
                    MessageBox.Show("¡Gracias por votar!");
                    NavigationService.Navigate(new Uri(string.Format("/pages/vote.xaml?idtopic=" + idtopic + "&random={0}" + "&nocache=" + Environment.TickCount, Guid.NewGuid()), UriKind.Relative));
                }
            });
            w.DownloadStringAsync(
            new Uri("http://localhost/beach/vote.php?user=" + data[0].Username + "&place=" + idtopic + "&vote=" + vote + "&comment=" + reason));
        }

        private void PhoneList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void createMessageBox(int vote)
        {
            PhoneTextBox content = new PhoneTextBox();
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Title = "¿Desea agregar un comentario?",
                Content = content,
                LeftButtonContent = "No",
                RightButtonContent = "Sí"
            };
            messageBox.Dismissed += (s2, e2) =>
            {
                switch (e2.Result)
                {
                    case CustomMessageBoxResult.RightButton:
                        saveVote(vote, content.Text);
                        break;
                    case CustomMessageBoxResult.LeftButton:
                        saveVote(vote, "");
                        break;
                }
            };
            messageBox.Show();
        }

        private void getComments()
        {
            WebClient w = new WebClient();

            string idtopic = (this.NavigationContext.QueryString["id"]);

            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
            .Subscribe(r =>
            {
                var deserialized = JsonConvert.DeserializeObject<List<resultReason>>(r.EventArgs.Result);

                reasonList.ItemsSource = deserialized;

            });
            w.DownloadStringAsync(
            new Uri("http://localhost/beach/comments.php?id=" + idtopic + "&r=" + Guid.NewGuid().ToString()));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                var total = data.Count;
                createMessageBox(1);
            }
            catch
            {
                MessageBox.Show("Necesita ingresar al sistema para votar, ¡Gracias!");
                return;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            try
            {
                var total = data.Count;
                createMessageBox(0);
            }
            catch
            {
                MessageBox.Show("Necesita ingresar al sistema para votar, ¡Gracias!");
                return;
            }
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            try
            {
                var total = data.Count;
                createMessageBox(2);
            }
            catch
            {
                MessageBox.Show("Necesita ingresar al sistema para votar, ¡Gracias!");
                return;
            }
        }
    }
}