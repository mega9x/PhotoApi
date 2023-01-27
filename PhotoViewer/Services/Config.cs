using Models.Config;
using Tomlyn;

namespace PhotoViewer.Services
{
    public class Config
    {
        public FontendConfig FontendConfig { get; init; }
        public Config()
        {
            if (!File.Exists(ConstStr.FontendConfig.GetConfigPath()))
            {
                FontendConfig = new FontendConfig();
                File.WriteAllText(ConstStr.FontendConfig.GetConfigPath(), Toml.FromModel(FontendConfig));
                return;
            }
            var toml = File.ReadAllText(ConstStr.FontendConfig.GetConfigPath());
            FontendConfig = Toml.ToModel<FontendConfig>(toml);
        }
    }
}
