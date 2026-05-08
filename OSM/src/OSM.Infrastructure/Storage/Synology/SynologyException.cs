namespace OSM.Infrastructure.Storage.Synology
{
    public sealed class SynologyException : Exception
    {
        public SynologyException(
            string message,
            string? responseBody = null,
            int? errorCode = null)
            : base(message)
        {
            ResponseBody = responseBody;
            ErrorCode = errorCode;
        }

        public string? ResponseBody { get; }

        public int? ErrorCode { get; }
    }
}
