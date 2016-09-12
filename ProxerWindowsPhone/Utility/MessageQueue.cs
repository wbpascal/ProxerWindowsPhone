using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Proxer.Utility
{
    public static class MessageQueue
    {
        private static Task _endlessTask;
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();
        private static readonly Queue<string> Messages = new Queue<string>();

        static MessageQueue()
        {
            StartTasks();
        }

        #region Properties

        public static TaskScheduler CurrentTaskScheduler { get; set; }

        #endregion

        #region Methods

        public static void AddMessage(string message)
        {
            Messages.Enqueue(message);
        }

        public static async Task CancelShowMessages()
        {
            if (!TokenSource.IsCancellationRequested) TokenSource.Cancel();
            await _endlessTask;
        }

        private static async Task ShowMessages()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                while (Messages.Count > 0)
                    await
                        await Task.Factory.StartNew(new MessageDialog(Messages.Dequeue()).ShowAsync, TokenSource.Token,
                            TaskCreationOptions.None, CurrentTaskScheduler ?? TaskScheduler.Default);
                await Task.Delay(100);
            }
        }

        private static async void StartTasks()
        {
            _endlessTask =
                await Task.Factory.StartNew(ShowMessages, TaskCreationOptions.LongRunning).ConfigureAwait(false);
        }

        #endregion
    }
}