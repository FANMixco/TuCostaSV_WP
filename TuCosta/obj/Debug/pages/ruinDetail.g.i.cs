﻿#pragma checksum "C:\Users\Federico Navarrete\documents\visual studio 2012\Projects\TuCosta\TuCosta\pages\ruinDetail.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "ADFFB0D975E8AFFFA4632F5B126B2916"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.33440
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace TuCosta.pages {
    
    
    public partial class ruinDetail : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock txtRuinName;
        
        internal System.Windows.Controls.Image location;
        
        internal System.Windows.Controls.TextBlock detail;
        
        internal Microsoft.Phone.Controls.Maps.Map map1;
        
        internal System.Windows.Controls.ListBox reasonList;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/TuCosta;component/pages/ruinDetail.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.txtRuinName = ((System.Windows.Controls.TextBlock)(this.FindName("txtRuinName")));
            this.location = ((System.Windows.Controls.Image)(this.FindName("location")));
            this.detail = ((System.Windows.Controls.TextBlock)(this.FindName("detail")));
            this.map1 = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("map1")));
            this.reasonList = ((System.Windows.Controls.ListBox)(this.FindName("reasonList")));
        }
    }
}

