namespace Models.Config.Updater
{
    public class ConfigRoot
    {
        public ApiConfig Api { get; set; } = new();
        public General General { get; set; } = new();
    }
}
