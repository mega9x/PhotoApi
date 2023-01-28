namespace ConstStr;

public class BackendConfig
{
    #region Root
    public const string CONFIG_ROOT = "./配置";
    public const string LIB_ROOT = "./结果";
    #endregion
    #region Images
    public const string ImgPath = $"{LIB_ROOT}/Images.json";
    public const string ArchivePath = $"{LIB_ROOT}/归档";
    public const string ArchiveFile = $"{ArchivePath}/images_date.json";

    #endregion
}