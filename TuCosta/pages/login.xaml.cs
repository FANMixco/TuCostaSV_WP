using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TuCosta.classes;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Microsoft.Phone.Reactive;
using System.IO;

namespace TuCosta.pages
{
    public partial class login : PhoneApplicationPage
    {
        public login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtPass.Password == "" || txtUser.Text == "")
            {
                MessageBox.Show("¡Debe llenar todos los campos!");
                return;
            }

            WebClient w = new WebClient();

            Observable
            .FromEvent<DownloadStringCompletedEventArgs>(w, "DownloadStringCompleted")
            .Subscribe(r =>
            {
                var deserialized = JsonConvert.DeserializeObject<result>(r.EventArgs.Result);
                if (deserialized.created == 0)
                    MessageBox.Show("¡Usuario o contraseña erronea!");
                else if (deserialized.created == 1)
                {
                    //Initialize the session here
                    // Write to the Isolated Storage
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("user.xml", FileMode.Create))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(List<UserInfo>));
                            using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                            {
                                serializer.Serialize(xmlWriter, GeneratePersonData());
                                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("¡Intente en otro momento!");
            });
            w.DownloadStringAsync(
            new Uri("http://locahost/beach/login.php?user=" + txtUser.Text + "&pass=" + txtPass.Password));

        }

        private List<UserInfo> GeneratePersonData()
        {
            List<UserInfo> data = new List<UserInfo>();
            UserInfo ui = new UserInfo();
            ui.Username = txtUser.Text;
            ui.Password = txtPass.Password;
            data.Add(ui);
            return data;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/pages/register.xaml", UriKind.Relative));
        }
    }
}