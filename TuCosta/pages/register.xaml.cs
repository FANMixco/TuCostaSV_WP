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
using System.Xml.Serialization;
using System.Xml;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Reactive;
using Newtonsoft.Json;

namespace TuCosta.pages
{
    public partial class register : PhoneApplicationPage
    {
        public register()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text == "" || txtUser.Text == "")
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
                if (deserialized.created == 3)
                    MessageBox.Show("¡El Usuario ya existe!");
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
            new Uri("http://diputado85.azurewebsites.net/hadlers/createUser.ashx?dui=" + txtUser.Text + "&name=" + txtName.Text));

        }

        private List<UserInfo> GeneratePersonData()
        {
            List<UserInfo> data = new List<UserInfo>();
            UserInfo ui = new UserInfo();
            ui.Username = txtUser.Text;
            ui.Password = txtUser.Text;
            data.Add(ui);
            return data;
        }
    }
}