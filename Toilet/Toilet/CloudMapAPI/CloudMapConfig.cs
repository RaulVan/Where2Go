using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toilet.CloudMapAPI
{
  internal  class CloudMapConfig
    {
        //POST
        public static string UrlCreate = "http://yuntuapi.amap.com/datamanage/data/create";
        public static string UrlBatchCreate = "http://yuntuapi.amap.com/datamanage/data/batchcreate";
        public static string UrlUpdate = "http://yuntuapi.amap.com/datamanage/data/update";
        public static string UrlDelete = "http://yuntuapi.amap.com/datamanage/ data/delete";
        //GET
        public static string UrlList = "http://yuntuapi.amap.com/datamanage/ data/list?";
        public static string UrlImportStatus = "http://yuntuapi.amap.com/datamanage/batch/importstatus?";


        public static string UrlAround = "http://yuntuapi.amap.com/datasearch/around?";
        public static string UrlPolygon = "http://yuntuapi.amap.com/datasearch/polygon?";
        public static string UrlId = "http://yuntuapi.amap.com/datasearch/id?";

    }
}
