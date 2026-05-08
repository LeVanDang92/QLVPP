namespace OSM.Application.Abstractions.Storage
{
    public interface IFileStorageService
    {
        Task CreateFolderAsync(
            string parentFolder,
            string folderName,
            CancellationToken cancellationToken = default);

        Task<string> UploadAsync(
            string folderPath,
            FileUpload file,
            bool overwrite = true,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        Task DownloadToStreamAsync(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = default);
    }
}
