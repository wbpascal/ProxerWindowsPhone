using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proxer
{
    public static class HttpUtility
    {
        #region

        public static async Task<string> GetRequest(Uri addressUri)
        {
            using (HttpClient lClient = new HttpClient())
            {
                return await lClient.GetStringAsync(addressUri);
            }
        }

        public static async Task<string> PostRequest(Uri addressUri, Dictionary<string, string> postArgs)
        {
            using (HttpClient lClient = new HttpClient())
            {
                return
                    await
                        (await lClient.PostAsync(addressUri, new FormUrlEncodedContent(postArgs))).Content
                            .ReadAsStringAsync();
            }
        }

        #endregion
    }
}