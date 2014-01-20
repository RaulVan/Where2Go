using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Toilet.Resources;
using Com.AMap.Maps.Api;

using Com.AMap.Api.Services.Results;
using Com.AMap.Api.Services;
using Com.AMap.Maps.Api.Overlays;
using System.Diagnostics;
using System.Threading.Tasks;
using Com.AMap.Api.Maps;
using Com.AMap.Maps.Api.BaseTypes;
using System.Windows.Media;
using Toilet.Control;



namespace Toilet
{
    public partial class MainPage : PhoneApplicationPage
    {

        MMap map;
        MapLayer mapLocationLayer;
        MapLayer mapToiletLayer;
        
        MMarker myLoaction;
        MLngLat mLngLat;
        MCircle mCircle;
        AMapGeolocator geo;
        ToiletTip tip;
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            this.mapPanel.Children.Add(map = new MMap());
            mapLocationLayer = new MapLayer();
            map.Children.Add(mapLocationLayer);
            mapToiletLayer = new MapLayer();
            map.Children.Add(mapToiletLayer);
            map.Zoom = 11d;
            map.ToolBar = Visibility.Visible;
            Com.AMap.Maps.Api.AMapConfig.Key = "1e4203f2ca6b055a0c24d1b41d772298";//
            AMapSearchConfig.Key = "1e4203f2ca6b055a0c24d1b41d772298";//
            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            this.Loaded += MainPage_Loaded;
            map.MapLoaded += map_MapLoaded;
           // map.Hold += map_Hold;

            Canvas.SetTop(btnLoaction, this.LayoutRoot.ActualHeight - 20);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AppConfig.IsOpenGeo)
            {
                MessageBoxResult msgResult = MessageBox.Show("需要开启定位", "运行定位", MessageBoxButton.OKCancel);
                if (msgResult.Equals(MessageBoxResult.OK))
                {
                    AppConfig.IsOpenGeo = true;//不再提示

                    geo = new AMapGeolocator();

                    geo.Start();
                    geo.PositionChanged += geo_PositionChanged;
                }
            }
            else
            {
                geo = new AMapGeolocator();

                geo.Start();
                geo.PositionChanged += geo_PositionChanged;
            }
        }

        /// <summary>
        /// 长按
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MLngLat lngLat= map.FromScreenPixelToLngLat(e.GetPosition(map));
            mCircle= new MCircle(lngLat, 1000);
            mCircle.FillOpacity = 0.3;
            mCircle.FillColor = Colors.Blue;
            mCircle.LineColor = Colors.Blue;
            mCircle.LineThickness = 2;
            mapToiletLayer.Children.Add(mCircle);
            HoldSearchGeoToAddress(lngLat);
            SearchKeyWordToilet(lngLat, 1000);
            //mapToiletLayer.SetFitview(GetMapMarker());
            
        }

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void map_MapLoaded(object sender, Com.AMap.Maps.Api.Events.MapEventArgs e)
        {
            //SearchKeyWordToilet();
            //MMarker editMarker;
            //map.Children.Add(editMarker = new MMarker()
            //{
            //    LngLat = map.Center,
            //    IsEditable = true,
            //    Anchor = new Point(0.5, 1)
            //});
           


        }
      
        /// <summary>
        /// 搜索厕所
        /// </summary>
        /// <param name="mlnglat"></param>
        /// <param name="radios"></param>
        private async void SearchKeyWordToilet(MLngLat mlnglat, uint radios)
        {
            //AMapFilterOption option=new AMapFilterOption ();
            //option.Groupbuy=false;
            //option.Discount=false;
            AMapPOIResults pois = await AMapPOISearch.POIAround(mlnglat.LngX, mlnglat.LatY, "公厕|厕所|洗手间|麦当劳|肯德基|必胜客|德克士", "", false, false, radios, 0, 100, 1, Extensions.All);
            this.Dispatcher.BeginInvoke(() =>
            {

                if (pois.Erro != null)
                {
                    return;
                }
                if (pois.POIList == null)
                {

                }
                List<MOverlay> list = new List<MOverlay>();
                foreach (AMapPOI poi in pois.POIList.ToList())
                {
                    //MMarker mm;
                    //map.Children.Add(mm = new MMarker()
                    //    {
                    //        LngLat = new Com.AMap.Maps.Api.BaseTypes.MLngLat(poi.Location.Lon, poi.Location.Lat),
                    //        IconURL = "/Image/AZURE.png"
                    //    });
                    MMarker Marker;
                    mapToiletLayer.Children.Add(Marker = new MMarker()
                    {
                        LngLat = new Com.AMap.Maps.Api.BaseTypes.MLngLat(poi.Location.Lon, poi.Location.Lat),
                        //IsEditable = true,
                        Anchor = new Point(0.5, 1),
                        IconURL = "Images/54x74.png",
                        // TipFrameworkElement = new TolietTip(){ TolietText = poi.Name }
                        TipFrameworkElement =tip=new ToiletTip () { Text = poi.Name,MarkerAMapPOI=poi },
                    });

                    tip.TapNavigation += tip_TapNavigation;
                    list.Add(Marker);
                    Debug.WriteLine(poi.Name.ToString());
                }
                map.SetFitview(list);

            });
        }

        void tip_TapNavigation(object sender, TapNavigationEventArgs e)
        {
            MessageBox.Show(e.MarkerAMapPOI.Address);
        }
        /// <summary>
        /// 定位按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if (geo == null)
            {
                mapToiletLayer.Children.Clear();
                geo = new AMapGeolocator();
                geo.Start();
                geo.PositionChanged += geo_PositionChanged;
            }
            else if(geo!=null)
            {
                geo.Start();
                geo.PositionChanged += geo_PositionChanged;
            }



        }
        /// <summary>
        /// 搜索按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplicationBarIconButton2_Click(object sender, EventArgs e)
        {
            if (myLoaction != null)
            {
                mapToiletLayer.Children.Clear();
                SearchGeoToAddress(myLoaction.LngLat);
                SearchKeyWordToilet(myLoaction.LngLat, 1000);
            }
            // SearchGeoToAddress(mLngLat);
            //SearchKeyWordToilet(mLngLat, 500);
        }
        /// <summary>
        /// 逆地理编码
        /// </summary>
        /// <param name="mlnglat"></param>
        private async void SearchGeoToAddress(MLngLat mlnglat)
        {
            AMapReGeoCodeResult rgrc = await AMapReGeoCodeSearch.GeoCodeToAddress(mlnglat.LngX, mlnglat.LatY);
            if (rgrc.Erro!=null)
            {
                return;
            } 
            if (rgrc.ReGeoCode!=null)
            {
            myLoaction.TipFrameworkElement = new MTip() { ContentText = rgrc.ReGeoCode.Formatted_address };
                
            }


        }
        /// <summary>
        /// 长按查找
        /// </summary>
        /// <param name="mlnglat"></param>
        private async void HoldSearchGeoToAddress(MLngLat mlnglat)
        {
            AMapReGeoCodeResult rgrc = await AMapReGeoCodeSearch.GeoCodeToAddress(mlnglat.LngX, mlnglat.LatY);

            mapToiletLayer.Children.Add(new MMarker() { LngLat = mlnglat, IconURL = "Images/AZURE.png", TipFrameworkElement = new MTip() { ContentText = rgrc.ReGeoCode.Formatted_address } });

        }

        private async void SearchWalking(MLngLat stat, MLngLat end)
        {
            AMapRouteResults rr = await AMapNavigationSearch.WalkingNavigation(stat.LngX, stat.LatY, end.LngX, end.LatY);
            if (rr.Erro!=null)
            {
                return;
            }
            if (rr.Route!=null&&rr.Count!=0)
            {
                AMapRoute route = rr.Route;
                List<AMapPath> paths = route.Paths.ToList();
                MLngLatCollection lnglats = new MLngLatCollection ();
                foreach (AMapPath path in paths)
                {
                    //画路线
                    List<AMapStep> steps = path.Steps.ToList();
                    foreach (AMapStep st in steps)
                    {

                        //amap.AddMarker(new AMapMarkerOptions()
                        //{
                        //    Position = latLagsFromString(st.Polyline).FirstOrDefault(),
                        //    Title = "Title",
                        //    Snippet = "Snippet",
                        //    IconUri = new Uri("Images/man.png", UriKind.Relative),
                        //});
                        Debug.WriteLine(st.Instruction);
                        Debug.WriteLine(st.Road);
                        Debug.WriteLine(st.Assistant_action);
                        lnglats = latLagsFromString(st.Polyline);
                       
                        MPolyline walkPolyling = new MPolyline(lnglats);
                        walkPolyling.LineThickness = 4;
                        walkPolyling.LineColor = Color.FromArgb(255, 0, 0, 255);
                        mapToiletLayer.Children.Add(walkPolyling);
                        //amap.AddPolyline(new AMapPolylineOptions()
                        //{
                        //    Points = latLagsFromString(st.Polyline),
                        //    Color = Color.FromArgb(255, 0, 0, 255),
                        //    Width = 4,
                        //});
                    }
                }
            }
        }
        private List<MOverlay> GetMapMarker()
        {
            List<MOverlay> list = new List<MOverlay>();
            foreach (var mOverlay in mapToiletLayer.Children)
            {
                if (mOverlay is MMarker)
                {
                    list.Add(mOverlay as MMarker);
                } 
            }
            return list;
        }

        private MLngLatCollection latLagsFromString(string polyline)
        {
            MLngLatCollection latlngs = new MLngLatCollection();

            string[] arrystring = polyline.Split(new char[] { ';' });
            foreach (String str in arrystring)
            {
                String[] lnglatds = str.Split(new char[] { ',' });
                latlngs.Add(new  MLngLat(Double.Parse(lnglatds[1]), Double.Parse(lnglatds[0])));
            }
            return latlngs;

        }

        void geo_PositionChanged(AMapGeolocator sender, AMapPositionChangedEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {

                if (myLoaction == null)
                {
                    myLoaction = new MMarker() { LngLat = new Com.AMap.Maps.Api.BaseTypes.MLngLat(args.LngLat.longitude, args.LngLat.latitude), IconURL = "/Images/location_on.png" };

                }
                if (!mapLocationLayer.Children.Contains(myLoaction))
                {
                   mapLocationLayer.Children.Add(myLoaction);
                }
                else
                {
                    myLoaction.LngLat = new Com.AMap.Maps.Api.BaseTypes.MLngLat(args.LngLat.longitude, args.LngLat.latitude);
                }
                map.Center = new Com.AMap.Maps.Api.BaseTypes.MLngLat(args.LngLat.longitude, args.LngLat.latitude);
                SearchKeyWordToilet(myLoaction.LngLat, 1000);
                // mLngLat = new Com.AMap.Maps.Api.BaseTypes.MLngLat(args.LngLat.longitude, args.LngLat.latitude);
                geo.PositionChanged -= geo_PositionChanged;
                geo.Stop();
                geo = null;
            });


        }

        private void Setting_Click(object sender, System.EventArgs e)
        {
        	// 在此处添加事件处理程序实现。
            //NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

      

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
            //if (!AppConfig.IsOpenGeo)
            //{
            //    MessageBoxResult msgResult = MessageBox.Show("需要开启定位", "运行定位", MessageBoxButton.OKCancel);
            //    if (msgResult.Equals(MessageBoxResult.OK))
            //    {
            //        AppConfig.IsOpenGeo = true;//不再提示

            //        geo = new AMapGeolocator();

            //        geo.Start();
            //        geo.PositionChanged += geo_PositionChanged;
            //    }
            //}
            //else
            //{
            //    geo = new AMapGeolocator();

            //    geo.Start();
            //    geo.PositionChanged += geo_PositionChanged;
            //}

           // base.OnNavigatedTo(e);
        //}

        //void watcher_PositionChanged(object sender, Com.AMap.Maps.Api.Events.AGeoPositionChangedEventArgs e)
        //{

        //}


        // 用于生成本地化 ApplicationBar 的示例代码
        //private void BuildLocalizedApplicationBar()
        //{
        //    // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
        //    ApplicationBar = new ApplicationBar();

        //    // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // 使用 AppResources 中的本地化字符串创建新菜单项。
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}