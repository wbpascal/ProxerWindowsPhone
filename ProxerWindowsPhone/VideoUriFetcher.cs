using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Proxer
{
    public static class VideoUriFetcher
    {
        #region

        private static async Task<Uri> GetDailymotionStreamUri(Uri baseUri, CancellationToken cancellationToken)
        {
            string lResponse = await HttpUtility.GetRequest(baseUri, cancellationToken);
            Match lFirstMatch = new Regex("\"(http[^\"]+?\\.mp4\\S+?)\"").Match(lResponse);
            if (lFirstMatch.Success) return new Uri(lFirstMatch.Captures[0].Value);
            throw new Exception();
        }

        private static async Task<Uri> GetMp4UploadStreamUri(Uri baseUri, CancellationToken cancellationToken)
        {
            string lResponse = await HttpUtility.GetRequest(baseUri, cancellationToken);
            Match lFirstMatch = new Regex(@"(http[s]*:\/\/www[0-9]+\S+?video\.mp4)").Match(lResponse);
            if (lFirstMatch.Success) return new Uri(lFirstMatch.Captures[0].Value);
            throw new Exception();
        }

        private static async Task<Uri> GetProxerStreamUri(Uri baseUri, CancellationToken cancellationToken)
        {
            string lResponse = await HttpUtility.GetRequest(baseUri, cancellationToken);
            Match lFirstMatch = new Regex(@"(http\S+?\.mp4)").Match(lResponse);
            if (lFirstMatch.Success) return new Uri(lFirstMatch.Captures[0].Value);
            throw new Exception();
        }

        private static async Task<Uri> GetStreamcloudStreamUri(Uri baseUri, CancellationToken cancellationToken)
        {
            string lResponse = await HttpUtility.GetRequest(baseUri, cancellationToken);
            MatchCollection lPostMatches =
                new Regex("input type=\"hidden\" name=\"(?<test1>\\S+?)\" value=\"(?<test2>\\S+?)\"").Matches(lResponse);
            Dictionary<string, string> lPostArgsDictionary = new Dictionary<string, string>();
            foreach (Match match in lPostMatches)
                lPostArgsDictionary.Add(match.Groups[1].Value, match.Groups[2].Value);

            await Task.Delay(TimeSpan.FromSeconds(11), cancellationToken);

            string lPostResponse = await HttpUtility.PostRequest(baseUri, lPostArgsDictionary, cancellationToken);
            Match lFirstMatch = new Regex(@"(http\S+?video\.mp4)").Match(lPostResponse);
            if (lFirstMatch.Success) return new Uri(lFirstMatch.Captures[0].Value);

            throw new Exception();
        }

        public static async Task HandleStreamPartnerUri(Uri baseUri, CancellationToken cancellationToken)
        {
            switch (baseUri.Host.Replace("www.", string.Empty))
            {
                case "stream.proxer.me":
                    StartVideoFromUri(await GetProxerStreamUri(baseUri, cancellationToken));
                    break;
                case "mp4upload.com":
                    StartVideoFromUri(await GetMp4UploadStreamUri(baseUri, cancellationToken));
                    break;
                case "streamcloud.eu":
                    StartVideoFromUri(await GetStreamcloudStreamUri(baseUri, cancellationToken));
                    break;
                case "dailymotion.com":
                    StartVideoFromUri(await GetDailymotionStreamUri(baseUri, cancellationToken));
                    break;
                default:
                    await
                        Task.Factory.StartNew(() => Launcher.LaunchUriAsync(baseUri), cancellationToken,
                            TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                    break;
            }
        }

        private static void StartVideoFromUri(Uri videoUri)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof(MediaPlayerPage), videoUri);
        }

        #endregion
    }
}