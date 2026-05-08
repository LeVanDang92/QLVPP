using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OSM.API.Options;
using OSM.Application.Abstractions.Storage;

namespace OSM.API.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    public sealed class FilesController(IFileStorageService fileStorageService, IOptions<FileUploadOptions> fileUploadOptions) : ApiController
    {

        [HttpPost("folders")]
        public async Task<IActionResult> CreateFolder(
            [FromBody] CreateFolderRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ParentFolder) || string.IsNullOrWhiteSpace(request.FolderName))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid folder",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "ParentFolder and FolderName are required."
                });
            }

            await fileStorageService.CreateFolderAsync(
                request.ParentFolder.Trim(),
                request.FolderName.Trim(),
                cancellationToken);

            return Ok();
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            [FromForm] UploadFileRequest request,
            CancellationToken cancellationToken)
        {
            if (request.File is null || request.File.Length == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid file",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "File is required."
                });
            }

            if (request.File.Length > fileUploadOptions.Value.MaxFileSizeBytes)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid file",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = $"File size must not exceed {fileUploadOptions.Value.MaxFileSizeBytes / 1024 / 1024} MB."
                });
            }

            if (string.IsNullOrWhiteSpace(request.FolderPath))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid folder",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "FolderPath is required."
                });
            }

            var safeFileName = Path.GetFileName(request.File.FileName);
            var contentType = string.IsNullOrWhiteSpace(request.File.ContentType)
                ? "application/octet-stream"
                : request.File.ContentType;

            await using var stream = request.File.OpenReadStream();

            var uploadedPath = await fileStorageService.UploadAsync(
                request.FolderPath.Trim(),
                new FileUpload(
                    stream,
                    safeFileName,
                    contentType,
                    request.File.Length),
                request.Overwrite,
                cancellationToken);

            return Ok(new UploadFileResponse(uploadedPath));
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(
            [FromQuery] string path,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid path",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Path is required."
                });
            }

            var stream = await fileStorageService.DownloadAsync(path.Trim(), cancellationToken);
            var fileName = Path.GetFileName(path);

            return File(stream, "application/octet-stream", fileName);
        }
    }

    public sealed record CreateFolderRequest(
        string ParentFolder,
        string FolderName);

    public sealed class UploadFileRequest
    {
        public string FolderPath { get; init; } = string.Empty;

        public IFormFile? File { get; init; }

        public bool Overwrite { get; init; } = true;
    }

    public sealed record UploadFileResponse(string Path);
}
