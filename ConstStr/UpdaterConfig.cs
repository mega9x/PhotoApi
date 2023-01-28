namespace ConstStr;

public class UpdaterConfig
{
    #region Root
    public string ApiRoot { get; init; } = "http://127.0.0.1";
    public const string ImageRoot = "./基底图片";
    public const string ConfigRoot = "./配置";
    #endregion
    #region FileName
    public const string ConfigFile = "配置.toml";
    #endregion
    #region Api
    public string UploadEndpoint { get; set; } = "/Photo/upload";
    public string UploadBucketEndpoint { get; set; } = "/photo/UploadByBucket";
    public string FetchSamplesEndpoint { get; init; } = "/Photo/GetSamples";
    #endregion
}