using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Proxer.Utility
{
    public static class HttpUtility
    {
        private static readonly HttpClient Client = new HttpClient();

        #region

        public static async Task<string> GetRequest(Uri address, CancellationToken cancellationToken)
        {
            HttpResponseMessage lResult = await Client.GetAsync(address, cancellationToken)
                .ConfigureAwait(false);
            return lResult.IsSuccessStatusCode
                ? await lResult.Content.ReadAsStringAsync().ConfigureAwait(false)
                : string.Empty;
        }

        public static async Task<string> PostRequest(Uri address, Dictionary<string, string> postArgs,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage lResult = await Client.PostAsync(address, new FormUrlEncodedContent(postArgs),
                cancellationToken).ConfigureAwait(false);
            return lResult.IsSuccessStatusCode
                ? await lResult.Content.ReadAsStringAsync().ConfigureAwait(false)
                : string.Empty;
        }

        public static async Task<byte[]> GetBytes(Uri address, CancellationToken cancellationToken)
        {
            HttpResponseMessage lResult = await Client.GetAsync(address, cancellationToken);
            return lResult.IsSuccessStatusCode
                ? await lResult.Content.ReadAsByteArrayAsync()
                : new byte[0];
        }

        #endregion
    }
}