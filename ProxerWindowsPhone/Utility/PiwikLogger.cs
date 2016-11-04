using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Proxer.Utility
{
    public class PiwikLogger
    {
        private const int PiwikSiteId = 1;
        private const string PiwikSiteUrl = "https://error.infinitesoul.me/";
        private const string PiwikUrl = "https://piwik.infinitesoul.me/";

        #region Methods

        private static string GetPiwikParameters(UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Exception lException = unhandledExceptionEventArgs.Exception;
            return $"idsite={PiwikSiteId}&rec=1" +
                   $"&url={PiwikSiteUrl}&action_name=ErrorReport&apiv=1" +
                   $"&c_n={lException.GetType().FullName}" +
                   $"&c_p={{\"version\": \"{typeof(App).GetTypeInfo().Assembly.GetName().Version}\", " +
                   $"\"unhandledMessage\": \"{unhandledExceptionEventArgs.Message}\", " +
                   $"\"exceptionMessage\": \"{lException.Message}\", " +
                   $"\"stacktrace\": \"{lException.StackTrace}\"}}" +
                   "&c_i=&send_image=0";
        }

        public static async Task LogUnhandledException(UnhandledExceptionEventArgs unhandledExceptionEventArgs,
            CancellationToken cancellationToken)
        {
            await HttpUtility.GetRequest(new Uri(
                    Uri.EscapeUriString($"{PiwikUrl}piwik.php?{GetPiwikParameters(unhandledExceptionEventArgs)}")),
                cancellationToken);
        }

        #endregion
    }
}