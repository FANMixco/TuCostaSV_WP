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
using Microsoft.Phone.Tasks;
using System.Threading;

namespace TuCosta.pages
{
    public partial class ruinDetail : PhoneApplicationPage
    {
        List<UserInfo> data;

        createMap cm;

        List<Tuple<double, double, string, int, int, bool>> locations = new List<Tuple<double, double, string, int, int, bool>>();

        public ruinDetail()
        {
            InitializeComponent();
        }

        private void loadMap(double latitude, double longitude)
        {
            WebClient wTemp = new WebClient();
            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(wTemp, "DownloadStringCompleted")
            .Subscribe(rTemp =>
            {
                var dTemp = JsonConvert.DeserializeObject<List<places>>(rTemp.EventArgs.Result);


                for (int i = 0; i < dTemp.Count; i++)
                    locations.Add(new Tuple<double, double, string, int, int, bool>(dTemp[i].latitude, dTemp[i].longitude, dTemp[i].place, dTemp[i].idplace, dTemp[i].idtype, false));

                cm.addPushpins(locations);
            });
            wTemp.DownloadStringAsync(
            new Uri("http://localhost/beach/places.php?lat=" + latitude + "&long=" + longitude));
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
                    Uri uri = new Uri(deserialized[0].image, UriKind.Relative);
                    location.Source = new BitmapImage(uri);

                    txtRuinName.Text = deserialized[0].place;

                    phone.Text = deserialized[0].phone;
                    
                    email.Text= deserialized[0].email;

                    detail.Text = deserialized[0].description;

                    website.Text = deserialized[0].website;

                    schedule.Text = deserialized[0].schedule;

                    extra.Text = deserialized[0].extra;

                    cm = new createMap(map1);

                    cm.setCenter(deserialized[0].latitude, deserialized[0].longitude, 14, true);

                    locations.Add(new Tuple<double, double, string, int, int, bool>(deserialized[0].latitude, deserialized[0].longitude, deserialized[0].place, deserialized[0].idplace, deserialized[0].idtype, true));

                    loadMap(deserialized[0].latitude, deserialized[0].longitude);
                });
                w.DownloadStringAsync(
                new Uri("http://localhost/beach/places.php?idplace=" + id));
            }
            else
                MessageBox.Show("No hay internet");

            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("user.xml", FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<UserInfo>));
                        data = (List<UserInfo>)serializer.Deserialize(stream);

                    }
                }
            }
            catch
            {
            }

        }

        private void menuProfile_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/login.xaml", UriKind.Relative));
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

        private void likeButton_Click(object sender, EventArgs e)
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

        private void dislikeButton_Click(object sender, EventArgs e)
        {
            try
            {
                var total = data.Count;
                createMessageBox(-1);
            }
            catch
            {
                MessageBox.Show("Necesita ingresar al sistema para votar, ¡Gracias!");
                return;
            }
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask
            {
                To = email.Text
            };

            emailComposeTask.Show();
        }

        private void TextBlock_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://" + website.Text);
            webBrowserTask.Show();
        }

        private void phone_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = phone.Text;
            phoneCallTask.DisplayName = txtRuinName.Text;

            phoneCallTask.Show();
        }

    }
}