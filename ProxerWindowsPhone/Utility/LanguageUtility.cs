using Azuria.Media.Properties;

namespace Proxer.Utility
{
    public static class LanguageUtility
    {
        #region Methods

        public static string GetChapterString(Language language)
        {
            switch (language)
            {
                case Language.English:
                    return "en";
                case Language.German:
                    return "de";
                default:
                    return string.Empty;
            }
        }

        public static Language GetFromChapterString(string lang)
        {
            switch (lang)
            {
                case "de":
                    return Language.German;
                case "en":
                    return Language.English;
                default:
                    return Language.Unkown;
            }
        }

        #endregion
    }
}