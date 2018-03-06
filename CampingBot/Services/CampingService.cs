using CampingBot.Model;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CampingBot.Services
{
    public class CampingService
    {
        private static string _campingAPIUrl = null;
        private static string GetCampingAPIUrl()
        {
            if (_campingAPIUrl == null)
                _campingAPIUrl = ConfigurationManager.AppSettings.Get("CampingAPIUrl");

            return _campingAPIUrl;
        }

        public static async Task<CampingInfo> GetCampingInfo()
        {
            HttpClient httpClient = new HttpClient();
            var result = await httpClient.GetAsync($"{GetCampingAPIUrl()}/camping");
            var jsonResult = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CampingInfo>(jsonResult);
        }
    }
}