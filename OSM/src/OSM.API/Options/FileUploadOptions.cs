namespace OSM.API.Options
{
    public sealed class FileUploadOptions
    {
        public const string SectionName = "FileUpload";

        public int MaxFileSizeMb { get; init; } = 50;

        public string[] AllowedExtensions { get; init; } = [];

        public long MaxFileSizeBytes => MaxFileSizeMb * 1024L * 1024L;
    }
}
