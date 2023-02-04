using Models.Config;
using Models.TrafficBackend.Config;
using TomlConfigManager;
using TrafficApi.Const;

namespace TrafficApi.Service
{
    public class ConfigService
    {
        public Config<ConfigRoot> Config { get; }

        public ConfigService()
        {
            Config = new ConfigBuilder()
                .UseProgramRoot()
                .SetConfigRoot(ConfigPath.ConfigRoot)
                .SetFileName(ConfigPath.ConfigName)
                .Build<ConfigRoot>();
        }
    }
}
