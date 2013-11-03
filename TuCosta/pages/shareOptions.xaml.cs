using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace TuCosta.pages
{
    public partial class shareOptions : PhoneApplicationPage
    {
        public shareOptions()
        {
            InitializeComponent();
        }

        private void ListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string name = this.NavigationContext.QueryString["name"];

            SmsComposeTask smsComposeTask = new SmsComposeTask();

            smsComposeTask.Body = "Te invito a conocer: " + name;

            smsComposeTask.Show();
        }

        private void ListBoxItem_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string name = this.NavigationContext.QueryString["name"];
            EmailComposeTask task = new EmailComposeTask();
            task.Subject = "Te invito a conocer: " + name;
            task.Show();
        }

        private void ListBoxItem_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string name = this.NavigationContext.QueryString["name"];

            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = "Te invito a conocer: " + name;

            shareStatusTask.Show();
        }
    }
}