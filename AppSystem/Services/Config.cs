using ConstStr;
using Models.Config.Fontend;
using Tomlyn;

namespace PhotoViewer.Services
{
    public class Config
    {
        public const string ConfigPath = $"{FontendConfigPath.ConfigRoot}/{FontendConfigPath.ConfigName}";
        private ConfigRoot ConfigRoot { get; set; }
        public Config()
        {
            if (!Directory.Exists(FontendConfigPath.ConfigRoot))
            {
                Directory.CreateDirectory(FontendConfigPath.ConfigRoot);
            }
            if (!File.Exists(FontendConfigPath.GetConfigPath()))
            {
                ConfigRoot = new ConfigRoot();
                File.WriteAllText(FontendConfigPath.GetConfigPath(), Toml.FromModel(ConfigRoot));
                return;
            }
            var toml = File.ReadAllText(FontendConfigPath.GetConfigPath());
            ConfigRoot = Toml.ToModel<ConfigRoot>(toml);
        }
        public Config Reload()
        {
            var configStr = File.ReadAllText(ConfigPath);
            ConfigRoot = Toml.ToModel<ConfigRoot>(configStr);
            return this;
        }
        public string GetApiRoot => ConfigRoot.PhotoApi.Api;
        public string GetPhotoEndpoint => ConfigRoot.PhotoApi.GetPhotoEndpoint;
        public string GetRandomPhotoEndpoint => ConfigRoot.PhotoApi.GetRandomPhotoEndpoint;
    }
}
