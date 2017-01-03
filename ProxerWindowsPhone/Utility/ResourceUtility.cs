using Windows.ApplicationModel.Resources;

namespace Proxer.Utility
{
    public static class ResourceUtility
    {
        private static readonly ResourceLoader ResourceLoader = new ResourceLoader();

        #region Methods

        public static string GetString(string resourceName)
        {
            return ResourceLoader.GetString(resourceName);
        }

        #endregion
    }
}