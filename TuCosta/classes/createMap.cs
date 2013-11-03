using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Input;
using System.Windows.Controls;
using TuCosta.Resources;

namespace TuCosta.classes
{
    public class Tuple<T1, T2, T3, T4, T5, T6>
    {
        public T1 Latitude { get; set; }
        public T2 Longitude { get; set; }
        public T3 Name { get; set; }
        public T4 idRuin { get; set; }
        public T5 idCountry { get; set; }
        public T6 now { get; set; }

        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            Latitude = item1;
            Longitude = item2;
            Name = item3;
            idRuin = item4;
            idCountry = item5;
            now = item6;
        }
    }

    class createMap
    {
        private Map mapLocation;
        private double Latitude;
        private double Longitude;
        private GeoCoordinateWatcher myCoordinateWatcher;

        public createMap(Map temp)
        {
            this.mapLocation = temp;
        }

        public void setCenter(double lat, double lon, double zoom, bool bar)
        {
            this.Latitude = lat;
            this.Longitude = lon;

            this.mapLocation.Center = new GeoCoordinate(this.Latitude, this.Longitude);
            this.mapLocation.ZoomLevel = zoom;
            if (bar == true)
                mapLocation.ZoomBarVisibility = Visibility.Visible;

            myCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            myCoordinateWatcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(this.myCoordinateWatcher_PositionChanged);

        }

        private void myCoordinateWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!e.Position.Location.IsUnknown)
            {
                Latitude = e.Position.Location.Latitude;
                Longitude = e.Position.Location.Longitude;
                mapLocation.Center = new GeoCoordinate(Latitude, Longitude);
            }
        }

        public void addPushpins(List<Tuple<double, double, string, int, int, bool>> Values)
        {
            foreach (Tuple<double, double, string, int, int, bool> location in Values)
            {

                ImageBrush ib = new ImageBrush();

                if (location.now == false)
                    ib.ImageSource =
                        new BitmapImage(new Uri("/images/icons/" + location.idCountry.ToString() + ".png", UriKind.Relative));
                else
                    ib.ImageSource =
                        new BitmapImage(new Uri("/images/icons/now/" + location.idCountry.ToString() + ".png", UriKind.Relative));

                Pushpin pp = new Pushpin();
                pp.Template = null;
                pp.Location = new GeoCoordinate(location.Latitude, location.Longitude);
                pp.Content = new Rectangle()
                {
                    Fill = ib,
                    Opacity = .8,
                    Height = 30,
                    Width = 26
                };

                mapLocation.Children.Add(pp); //myMap is your map control
                pp.MouseLeftButtonUp += getHandler(location);
                pp.Tag = location;
            }
        }
        public MouseButtonEventHandler getHandler(Tuple<double, double, string, int, int, bool> cls)
        {
            return delegate(object sender, MouseButtonEventArgs e)
            {

                string url = "/pages/ruinDetail.xaml?id=" + cls.idRuin.ToString();

                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = AppResources.Learn.ToString(),
                    Message = cls.Name,
                    LeftButtonContent = AppResources.Ok.ToString(),
                    RightButtonContent = AppResources.View.ToString()
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            break;
                        case CustomMessageBoxResult.RightButton:
                            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(url, UriKind.Relative));
                            break;
                        case CustomMessageBoxResult.None:
                            break;
                        default:
                            break;
                    }
                };
                messageBox.Show();
            };
        }
    }
}
