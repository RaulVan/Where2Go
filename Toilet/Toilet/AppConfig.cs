﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace Toilet
{
   public static class AppConfig
    {
       public static string AppKey = "5321d66856240b031503e8ff";

       public static string DebugAppKey = "5313e59056240b7a8a1ab50e";

       /// <summary>
       /// 是否开启定位
       /// </summary>
       public static bool IsOpenGeo
       {
           get
           {

               return IsolatedStorageSettings.ApplicationSettings.Contains("IsOpenGeo") ? (bool)IsolatedStorageSettings.ApplicationSettings["IsOpenGeo"] : false;
           }
           set
           {
               IsolatedStorageSettings.ApplicationSettings["IsOpenGeo"] = value;
               IsolatedStorageSettings.ApplicationSettings.Save();
           }
       }
       /// <summary>
       /// 搜索半径
       /// </summary>
       public static int Radius
       {
           get;
           set;
           //get
           //{
           //    if (IsolatedStorageSettings.ApplicationSettings.Contains("Radius"))
           //    {
           //        return  IsolatedStorageSettings.ApplicationSettings["Radius"] 
           //    }

           //}
       }


    }
}
