namespace ConstStr
{
    public class FontendConfigPath
    {
        #region Root
        public const string ConfigRoot = "./config";
        #endregion
        public const string ConfigName = "config.toml";
        /// <summary>
        /// 用于获取配置文件完整路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigPath() => Path.GetFullPath($"{ConfigRoot}/{ConfigName}");
    }
}
