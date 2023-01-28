using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstStr;
using Tomlyn;

namespace ImageUpdater
{
    public class Config
    {
        private const string ConfigPath = $"{UpdaterConfig.ConfigRoot}/{UpdaterConfig.ConfigFile}";
        public static Config Instance = new Lazy<Config>(() => new Config()).Value;
        public UpdaterConfig UpdaterConfig { get; private set; }
        private Config()
        {
            if (!Directory.Exists(UpdaterConfig.ImageRoot))
            {
                Directory.CreateDirectory(UpdaterConfig.ImageRoot);
            }
            if (!Directory.Exists(UpdaterConfig.ConfigRoot))
            {
                Directory.CreateDirectory(UpdaterConfig.ConfigRoot);
            }
            if (!File.Exists(ConfigPath))
            {
                var configString = Toml.FromModel(new UpdaterConfig());
                File.WriteAllText(ConfigPath, configString);
            }
            var configStr = File.ReadAllText(ConfigPath);
            UpdaterConfig = Toml.ToModel<UpdaterConfig>(configStr);
        }

        public Config Reload()
        {
            var configStr = File.ReadAllText(ConfigPath);
            UpdaterConfig = Toml.ToModel<UpdaterConfig>(configStr);
            return this;
        }
    }
}
