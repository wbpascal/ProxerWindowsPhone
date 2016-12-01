using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Proxer.Utility
{
    public static class HttpUtility
    {
        #region

        public static async Task<string> GetRequest(Uri addressUri, CancellationToken cancellationToken)
        {
            using (HttpClient lClient = new HttpClient())
            {
                HttpResponseMessage lResult = await lClient.GetAsync(addressUri, cancellationToken)
                    .ConfigureAwait(false);
                return lResult.IsSuccessStatusCode
                    ? await lResult.Content.ReadAsStringAsync().ConfigureAwait(false)
                    : string.Empty;
            }
        }

        public static async Task<string> PostRequest(Uri addressUri, Dictionary<string, string> postArgs,
            CancellationToken cancellationToken)
        {
            using (HttpClient lClient = new HttpClient())
            {
                HttpResponseMessage lResult =
                    await lClient.PostAsync(addressUri, new FormUrlEncodedContent(postArgs), cancellationToken)
                        .ConfigureAwait(false);
                return lResult.IsSuccessStatusCode
                    ? await lResult.Content.ReadAsStringAsync().ConfigureAwait(false)
                    : string.Empty;
            }
        }

        #endregion
    }
}