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

namespace Toilet
{
    public partial class SettingPage : PhoneApplicationPage
    {
        public SettingPage()
        {
            InitializeComponent();
            if (AppConfig.IsOpenGeo)
            {
                togLocaOffandOn.IsChecked = true;
            }
            else
            {
                togLocaOffandOn.IsChecked = false;
            }
            togLocaOffandOn.Checked+=togLocaOffandOn_Checked;
            togLocaOffandOn.Unchecked += togLocaOffandOn_Unchecked;
        }

       
        /// <summary>
        /// Email 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Where2Go反馈";
           
            emailComposeTask.To = "vanraul@outlook.com";
            // emailComposeTask.CodePage // 默认为 utf-8

            emailComposeTask.Show();

        }

        private void togLocaOffandOn_Checked(object sender, RoutedEventArgs e)
        {
            togLocaOffandOn.Content = "开启定位";
            AppConfig.IsOpenGeo = true;

        }

        void togLocaOffandOn_Unchecked(object sender, RoutedEventArgs e)
        {
            togLocaOffandOn.Content = "关闭定位";
            AppConfig.IsOpenGeo = false;
        }

        private void hyBtnPrivacy_Click(object sender, RoutedEventArgs e)
        {
            string str = "1.当前定位仅用于该应用的定位服务，不作他用";
            str += Environment.NewLine;
            str += "2.你可以在设置中启用或者关闭定位服务";
            str += Environment.NewLine;
            str += "3.使用的地图资源以及服务所有权归高德地图所拥有";
            str += Environment.NewLine;

            MessageBox.Show(str,"隐私策略",MessageBoxButton.OK);
        }
    }
}