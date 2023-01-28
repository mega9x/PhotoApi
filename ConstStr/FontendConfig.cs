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
        public const string ConfigRoot = "./config";
        #endregion
        public const string ConfigName = "config.toml";
        public static string GetConfigPath() => Path.GetFullPath($"{ConfigRoot}/{ConfigName}");
    }
}
