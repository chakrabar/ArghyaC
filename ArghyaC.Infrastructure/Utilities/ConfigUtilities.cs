using System.Configuration;

namespace ArghyaC.Infrastructure.Utilities
{
    public class ConfigUtilities
    {
        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool GetAppSettingsBool(string key)
        {
            var result = false;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out result) ? result : false;
        }
    }
}