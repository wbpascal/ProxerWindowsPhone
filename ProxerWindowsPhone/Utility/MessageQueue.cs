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
        private static readonly Queue<string> Messages = new Queue<string>();
        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();

        #region Properties

        private static TaskScheduler CurrentTaskScheduler { get; set; }

        #endregion

        #region Methods

        public static void AddMessage(string message)
        {
            Messages.Enqueue(message);
        }

        public static async Task CancelQueue()
        {
            if (!TokenSource.IsCancellationRequested) TokenSource.Cancel();
            await _endlessTask;
        }

        public static void Initialise(TaskScheduler taskScheduler)
        {
            CurrentTaskScheduler = taskScheduler;
            StartTasks();
        }

        private static async Task ShowMessages()
        {
            while (!TokenSource.IsCancellationRequested)
            {
                while ((Messages.Count > 0) && (CurrentTaskScheduler != null))
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