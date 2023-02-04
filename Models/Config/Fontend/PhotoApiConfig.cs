namespace Models.Config.Fontend
{
    public class PhotoApiConfig
    {
        public string Api { get; set; } = "http://127.0.0.1";
        public string GetPhotoEndpoint { get; set; } = "/photo/getphoto";
        public string GetPhotoGroupEndpoint { get; set; } = "/photo/getphotogroup";
        public string GetRandomPhotoEndpoint { get; set; } = "/photo/getrandomphoto";
    }
}
