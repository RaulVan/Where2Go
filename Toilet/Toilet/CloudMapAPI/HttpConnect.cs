using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Toilet.CloudMapAPI
{
   internal class HttpConnect
    {
       public static async Task<string> PostStringAsync(string data, string uri, string contenType)
       {
           HttpClient client = new HttpClient();
           HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);


           request.Content = new StringContent(data, Encoding.UTF8, contenType);
           HttpResponseMessage response = await client.SendAsync(request);
           string responseString = await response.Content.ReadAsStringAsync();
           return responseString;
       }

       public static async Task<string> GetStringAsync(string data, string uri)
       {
           HttpClient client = new HttpClient();
           HttpResponseMessage response = await client.GetAsync(string.Format("{0}{1}", uri, data));
           string responseString = await response.Content.ReadAsStringAsync();
           //TODO:判断返回的status，


           return responseString;
       }

    }
}
