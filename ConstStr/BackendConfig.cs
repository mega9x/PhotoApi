namespace ConstStr;

public class BackendConfig
{
    #region Root
    public const string CONFIG_ROOT = "./配置";
    public const string LIB_ROOT = "./结果";
    #endregion
    #region Images
    public const string IMG_PATH = $"{LIB_ROOT}/Images.json";
    public const string ARCHIVE_PATH = $"{LIB_ROOT}/归档";
    public const string ARCHIVE_FILE = $"{ARCHIVE_PATH}/images_date.json";

    #endregion
}