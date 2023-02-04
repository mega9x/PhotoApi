namespace ConstStr.Api
{
    public class Llinks
    {
        public static readonly string Root = "https://llinks.io";

        public static readonly string BaseEndpoint = "/member";
        public static readonly string LoginEndpoint = "/member/?cmd=member&action=login";
        public static readonly string GroupDetailsEndpoint = "/member/?cmd=member&action=group-details&group-id=";
        public static readonly string GroupsTableEndpoint = "/member/index.php?cmd=member&action=group-list";
    }
}
