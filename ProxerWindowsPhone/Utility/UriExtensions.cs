using System;
using System.Text;

namespace Proxer.Utility
{
    public static class UriExtensions
    {
        #region Methods

        public static Uri AddQueryParam(this Uri source, string paramKey, string paramValue)
        {
            UriBuilder lUriBuilder = new UriBuilder(source);
            StringBuilder lBuilder = new StringBuilder(lUriBuilder.Query);
            lBuilder.Append(string.IsNullOrEmpty(lUriBuilder.Query.Trim())
                ? $"?{paramKey}={paramValue}"
                : $"&{paramKey}={paramValue}");
            lUriBuilder.Query = lBuilder.ToString().Remove(0, 1);
            return lUriBuilder.Uri;
        }

        #endregion
    }
}