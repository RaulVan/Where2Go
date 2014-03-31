using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toilet.CloudMapAPI
{
    public class CloudMap
    {
        //上传数据

        public static async Task<string> CreateDataAsync(CloudOptions options)
        {

            return await HttpConnect.PostStringAsync(options.Datas, CloudMapConfig.UrlCreate, "application/x-www-form-urluncoded");
        }

        //查询数据

        //修改数据


    }
}
