namespace Models.Config.Fontend
{
    public class ApiConfig
    {
        public string ApiRoot { get; init; } = "http://127.0.0.1";
        public string GetPhotoEndpoint { get; init; } = "/photo/getphoto";
        public string GetRandomPhotoEndpoint { get; init; } = "/photo/getrandomphoto";
    }
}
