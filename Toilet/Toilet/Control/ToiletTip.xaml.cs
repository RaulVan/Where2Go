using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Com.AMap.Maps.Api.BaseTypes;
using Com.AMap.Api.Services.Results;

namespace Toilet.Control
{
    public partial class ToiletTip : UserControl
    {
        public event EventHandler< TapNavigationEventArgs> TapNavigation;
        public ToiletTip()
        {
            InitializeComponent();
            this.LayoutRoot.Width = this.Width;
            this.LayoutRoot.Height = this.Height;
        }


        public string Text
        {
            get 
            {
                return ShowText.Text;
            }
            set
            {
                ShowText.Text = value;
            }
        }

        public MLngLat MarkerPoint
        {
            get;
            set;
        }

        public AMapPOI MarkerAMapPOI
        {
            get;
            set;
        }
        private void btnNavi_Click(object sender, RoutedEventArgs e)
        {
            if (TapNavigation!=null)
            {
                TapNavigation(this, new TapNavigationEventArgs() { MarkerAMapPOI=MarkerAMapPOI});
            }
        }
    }
}
