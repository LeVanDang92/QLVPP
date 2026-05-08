namespace OSM.Application.Abstractions.Storage
{
    public sealed class FileUpload
    {
        public FileUpload(
            Stream content,
            string fileName,
            string? contentType = null,
            long? length = null)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            FileName = Path.GetFileName(fileName ?? throw new ArgumentNullException(nameof(fileName)));
            ContentType = string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType;
            Length = length;
        }

        public Stream Content { get; }

        public string FileName { get; }

        public string ContentType { get; }

        public long? Length { get; }
    }
}
