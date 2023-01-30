namespace Models.Config.Updater
{
    public class ApiConfig
    {
        public string ApiRoot { get; set; } = "http://127.0.0.1";
        #region Api
        public string UploadEndpoint { get; set; } = "/Photo/upload";
        public string UploadBucketEndpoint { get; set; } = "/photo/UploadByBucket";
        public string FetchSamplesEndpoint { get; init; } = "/Photo/GetSamples";
        #endregion
    }
}
