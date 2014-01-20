using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Toilet
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();
            if (AppConfig.IsOpenGeo)
            {
                this.ckIsActiveGPS.IsChecked = true;
            }
			else
			{
				this.ckIsActiveGPS.IsChecked=false;
			}
        }

        private void ckIsActiveGPS_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            if ((bool)(sender as CheckBox).IsChecked)
            {
                AppConfig.IsOpenGeo = true;
            }
			else
			{
				AppConfig.IsOpenGeo=false;
			}

        }

    }
}