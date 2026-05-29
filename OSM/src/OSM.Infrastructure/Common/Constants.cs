namespace OSM.Infrastructure.Common
{
    public static class Constants
    {
        public const string MENU_ID_00 = "dashboard";
        public const string MENU_TYPE_M = "M";
        public const string MENU_TYPE_C = "C";
        public const string MENU_GROUP= "BAS";

        public const string API_AUTH_DOMAIN = "/api/auth";

        public const string REFRESH_TOKEN = "refreshToken";
    }

    public enum PermissionEnum
    {
        read ,
        write,
        delete
    }
}
