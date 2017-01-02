using System.Threading.Tasks;

namespace Proxer.Utility
{
    public static class TaskExtensions
    {
        #region Methods

        public static T WaitAndReturn<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        public static void WaitTaskFactory(this Task<Task> factoryTask)
        {
            factoryTask.WaitAndReturn().Wait();
        }

        public static void WaitTaskFactory<T>(this Task<Task<T>> factoryTask)
        {
            factoryTask.WaitAndReturn().Wait();
        }

        #endregion
    }
}