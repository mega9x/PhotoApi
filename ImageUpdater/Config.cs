using ConstStr;
using Models.Config.Updater;
using Tomlyn;

namespace ImageUpdater
{
    public class Config
    {
        private const string ConfigPath = $"{UpdaterConfigPath.ConfigRoot}/{UpdaterConfigPath.ConfigFile}";
        public static Config Instance = new Lazy<Config>(() => new Config()).Value;
        private ConfigRoot Root { get; set; }
        private Config()
        {
            if (!Directory.Exists(UpdaterConfigPath.ImageRoot))
            {
                Directory.CreateDirectory(UpdaterConfigPath.ImageRoot);
            }
            if (!Directory.Exists(UpdaterConfigPath.ConfigRoot))
            {
                Directory.CreateDirectory(UpdaterConfigPath.ConfigRoot);
            }
            if (!File.Exists(ConfigPath))
            {
                var configString = Toml.FromModel(new ConfigRoot());
                File.WriteAllText(ConfigPath, configString);
            }
            var configStr = File.ReadAllText(ConfigPath);
            Root = Toml.ToModel<ConfigRoot>(configStr);
        }
        public Config Reload()
        {
            var configStr = File.ReadAllText(ConfigPath);
            Root = Toml.ToModel<ConfigRoot>(configStr);
            return this;
        }
        public string GetApiRoot => Root.Api.ApiRoot;
        public string GetUploadEndpoint => Root.Api.UploadEndpoint;
        public string GetFetchSamplesEndpoint => Root.Api.FetchSamplesEndpoint;
        public string GetUploadBucketEndpoint => Root.Api.UploadBucketEndpoint;
    }
}
