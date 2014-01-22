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

        MMap map;//地图
        MapLayer mapLocationLayer;//定位图层
        MapLayer mapToiletLayer;//卫生间点图层
        MapLayer mapWalkingLayer;//步行路径图层
        MMarker myLoaction;//定位点
        MCircle mCircle;
        AMapGeolocator geo;//用于高德地图的定位
        ToiletTip tip;//弹窗
        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            this.mapPanel.Children.Add(map = new MMap());
            mapLocationLayer = new MapLayer();
            map.Children.Add(mapLocationLayer);
            map.Children.Add(mapWalkingLayer = new MapLayer());
            mapToiletLayer = new MapLayer();
            map.Children.Add(mapToiletLayer);
            map.Zoom = 11d;
            map.ToolBar = Visibility.Visible;

            // 用于本地化 ApplicationBar 的示例代码
            //BuildLocalizedApplicationBar();
            this.Loaded += MainPage_Loaded;
            map.MapLoaded += map_MapLoaded;
            // map.Hold += map_Hold;
            Canvas.SetTop(btnLoaction, this.LayoutRoot.ActualHeight - 20);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Debug.WriteLine("MainPage_Loaded:" + ex.Message);
            }
        }

        ///// <summary>
        ///// 长按
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void map_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    MLngLat lngLat = map.FromScreenPixelToLngLat(e.GetPosition(map));
        //    mCircle = new MCircle(lngLat, 1000);
        //    mCircle.FillOpacity = 0.3;
        //    mCircle.FillColor = Colors.Blue;
        //    mCircle.LineColor = Colors.Blue;
        //    mCircle.LineThickness = 2;
        //    mapToiletLayer.Children.Add(mCircle);
        //    HoldSearchGeoToAddress(lngLat);
        //    SearchKeyWordToilet(lngLat, 1000);
        //    //mapToiletLayer.SetFitview(GetMapMarker());
        //}

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void map_MapLoaded(object sender, Com.AMap.Maps.Api.Events.MapEventArgs e)
        {
            
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

            try
            {
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
                            TipFrameworkElement = tip = new ToiletTip() { Text = poi.Name, MarkerAMapPOI = poi },
                        });

                        tip.TapNavigation += tip_TapNavigation;
                        list.Add(Marker);
                        Debug.WriteLine(poi.Name.ToString());
                    }
                    map.SetFitview(list);

                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SearchKeyWordToilet:" + ex.Message);
            }
        }

        void tip_TapNavigation(object sender, TapNavigationEventArgs e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(() =>
                        {
                            MLngLat end = new MLngLat(e.MarkerAMapPOI.Location.Lon, e.MarkerAMapPOI.Location.Lat);
                            mapWalkingLayer.Children.Clear();
                            SearchWalking(myLoaction.LngLat, end);
                        });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("tip_TapNavigation:" + ex.Message);   
            }
        }
       
        /// <summary>
        /// 逆地理编码
        /// </summary>
        /// <param name="mlnglat"></param>
        private async void SearchGeoToAddress(MLngLat mlnglat)
        {
            try
            {
                AMapReGeoCodeResult rgrc = await AMapReGeoCodeSearch.GeoCodeToAddress(mlnglat.LngX, mlnglat.LatY);
                if (rgrc.Erro != null)
                {
                    return;
                }
                if (rgrc.ReGeoCode != null)
                {
                    myLoaction.TipFrameworkElement = new MTip() { ContentText = rgrc.ReGeoCode.Formatted_address };

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SearchGeoToAddress:" + ex.Message);
            }


        }
        /// <summary>
        /// 长按查找
        /// </summary>
        /// <param name="mlnglat"></param>
        [Obsolete]
        private async void HoldSearchGeoToAddress(MLngLat mlnglat)
        {
            try
            {

                AMapReGeoCodeResult rgrc = await AMapReGeoCodeSearch.GeoCodeToAddress(mlnglat.LngX, mlnglat.LatY);

                mapToiletLayer.Children.Add(new MMarker() { LngLat = mlnglat, IconURL = "Images/AZURE.png", TipFrameworkElement = new MTip() { ContentText = rgrc.ReGeoCode.Formatted_address } });

            }
            catch (Exception ex)
            {
                Debug.WriteLine("HoldSearchGeoToAddress:" + ex.Message);
            }
        }

        private async void SearchWalking(MLngLat stat, MLngLat end)
        {
            try
            {
                AMapRouteResults rr = await AMapNavigationSearch.WalkingNavigation(stat.LngX, stat.LatY, end.LngX, end.LatY);
                if (rr.Erro != null)
                {
                    return;
                }
                if (rr.Route != null && rr.Count != 0)
                {
                    AMapRoute route = rr.Route;
                    List<AMapPath> paths = route.Paths.ToList();
                    MLngLatCollection lnglats = new MLngLatCollection();
                    List<MOverlay> list = new List<MOverlay>();
                    foreach (AMapPath path in paths)
                    {
                        //画路线
                        List<AMapStep> steps = path.Steps.ToList();
                        foreach (AMapStep st in steps)
                        {
                            Debug.WriteLine(st.Instruction);
                            Debug.WriteLine(st.Road);
                            Debug.WriteLine(st.Assistant_action);
                            lnglats = latLagsFromString(st.Polyline);

                            MPolyline walkPolyling = new MPolyline(lnglats);
                            walkPolyling.LineThickness = 6;
                            walkPolyling.LineColor = Colors.Green;
                            mapWalkingLayer.Children.Add(walkPolyling);
                            list.Add(walkPolyling);
                        }
                    }
                    map.SetFitview(list);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SearchWalking:" + ex.Message);
            }
        }
       
        /// <summary>
        /// 经纬度字符串转成经纬度集合
        /// </summary>
        /// <param name="polyline"></param>
        /// <returns></returns>
        private MLngLatCollection latLagsFromString(string polyline)
        {
            MLngLatCollection latlngs = new MLngLatCollection();

            string[] arrystring = polyline.Split(new char[] { ';' });
            foreach (String str in arrystring)
            {
                String[] lnglatds = str.Split(new char[] { ',' });
                latlngs.Add(new MLngLat(Double.Parse(lnglatds[0]), Double.Parse(lnglatds[1])));
            }
            return latlngs;

        }

        /// <summary>
        /// 定位位置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void geo_PositionChanged(AMapGeolocator sender, AMapPositionChangedEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                try
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
                   
                    SearchKeyWordToilet(myLoaction.LngLat, 1000);//搜索，默认半径1000m
               
                    geo.PositionChanged -= geo_PositionChanged;
                    geo.Stop();
                    geo = null;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("geo_PositionChanged:" + ex.Message);
                }
            });


        }

        private void Setting_Click(object sender, System.EventArgs e)
        {
            // 在此处添加事件处理程序实现。
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 定位按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoaction_Click(object sender, RoutedEventArgs e)
        {
            //TODO:重新定位后的操作，定位+搜索,应该需要删除之前的点
            try
            {
                mapToiletLayer.Children.Clear();
                mapWalkingLayer.Children.Clear();

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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}