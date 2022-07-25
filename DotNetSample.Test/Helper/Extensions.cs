using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetSample.Test.Helper
{
    public static class Extensions
    {
        public static async Task<Tout> ReadAsAsync<Tout>(this System.Net.Http.HttpContent content)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Tout>(await content.ReadAsStringAsync());
        }
    }
}
