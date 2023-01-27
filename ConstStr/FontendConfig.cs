using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstStr
{
    public class FontendConfig
    {
        #region Root
        public const string CONFIG_ROOT = "./config";
        #endregion
        public const string CONFIG_NAME = "config.toml";
        public static string GetConfigPath() => Path.GetFullPath($"{CONFIG_ROOT}/{CONFIG_NAME}");
    }
}
