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
using Microsoft.Live;
using System.Windows.Media.Imaging;

namespace TuCosta.pages
{
    public partial class login : PhoneApplicationPage
    {
        private LiveConnectClient liveClient = null;
        private string userImage;
        public string ProfileImage
        {
            get
            {
                return userImage;
            }
            set
            {
                userImage = value;

            }
        }

        string uid;

        private static LiveConnectSession session = null;
        public static LiveConnectSession Session
        {
            get
            {
                return session;
            }
            set
            {
                session = value;
            }
        }

        // Constructor
        public login()
        {
            InitializeComponent();
        }

        private void SignInButton_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (e != null && e.Session != null && e.Status == LiveConnectSessionStatus.Connected)
            {
                this.liveClient = new LiveConnectClient(e.Session);
                Session = e.Session;
                this.LoginIn();
            }
            else
                LoginOut();
        }

        private void LoginOut()
        {

            textBlockHeader.Visibility = Visibility.Collapsed;
            textBlockBtitle.Visibility = Visibility.Collapsed;

            textBlockBday.Text = "";
            textBlockName.Text = "";
            imageUser.Source = null;

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            storage.DeleteFile("user.xml");
        }

        private void LoginIn()
        {
            this.GetUserProfile();
        }

        private void createUserInfo(object sender, LiveOperationCompletedEventArgs e)
        {
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
                    }
                }
            }
        }

        private void GetUserProfile()
        {
            LiveConnectClient clientGetMe = new LiveConnectClient(Session);
            clientGetMe.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(clientGetMe_GetCompleted);
            clientGetMe.GetAsync("me", null);

            LiveConnectClient clientGetPicture = new LiveConnectClient(Session);
            clientGetPicture.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(clientGetPicture_GetCompleted);
            clientGetPicture.GetAsync("me/picture");

            LiveConnectClient clientGetInfo = new LiveConnectClient(Session);
            clientGetMe.GetCompleted += new EventHandler<LiveOperationCompletedEventArgs>(createUserInfo);
            clientGetMe.GetAsync("me", null);
        }

        void clientGetPicture_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                var uimage = (string)e.Result["location"];

                imageUser.Source = new BitmapImage(new Uri(uimage, UriKind.RelativeOrAbsolute));
            }
        }

        void clientGetMe_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                textBlockName.Text = (string)e.Result["name"];

                uid = (string)e.Result["id"];

                LiveConnectClient readBday = new LiveConnectClient(Session);
                readBday.GetCompleted +=
                     new EventHandler<LiveOperationCompletedEventArgs>(ReadBday_GetCompleted);
                readBday.GetAsync(uid);
            }
        }

        void ReadBday_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string bday = e.Result["birth_day"].ToString();
                string bmonth = e.Result["birth_month"].ToString();
                string bMonthString = "";

                //convert month
                switch (bmonth)
                {
                    case "1":
                        bMonthString = "January";
                        break;
                    case "2":
                        bMonthString = "February";
                        break;
                    case "3":
                        bMonthString = "March";
                        break;
                    case "4":
                        bMonthString = "April";
                        break;
                    case "5":
                        bMonthString = "May";
                        break;
                    case "6":
                        bMonthString = "June";
                        break;
                    case "7":
                        bMonthString = "July";
                        break;
                    case "8":
                        bMonthString = "August";
                        break;
                    case "9":
                        bMonthString = "September";
                        break;
                    case "10":
                        bMonthString = "October";
                        break;
                    case "11":
                        bMonthString = "November";
                        break;
                    case "12":
                        bMonthString = "December";
                        break;
                    default:
                        break;
                }

                // assign the textblock
                textBlockBday.Text = bday + "," + " " + bMonthString;
                textBlockHeader.Visibility = Visibility.Visible;
                textBlockBtitle.Visibility = Visibility.Visible;

            }
        }

        private List<UserInfo> GeneratePersonData()
        {
            List<UserInfo> data = new List<UserInfo>();
            UserInfo ui = new UserInfo();
            ui.Username = uid;

            data.Add(ui);
            return data;
        }
    }
}