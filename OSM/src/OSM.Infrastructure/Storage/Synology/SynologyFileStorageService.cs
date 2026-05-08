using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OSM.Application.Abstractions.Storage;

namespace OSM.Infrastructure.Storage.Synology
{
    public sealed class SynologyFileStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly SynologyOptions _options;
        private readonly ILogger<SynologyFileStorageService> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SynologyFileStorageService(
            HttpClient httpClient,
            IOptions<SynologyOptions> options,
            ILogger<SynologyFileStorageService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task CreateFolderAsync(
            string parentFolder,
            string folderName,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(parentFolder);
            ArgumentException.ThrowIfNullOrWhiteSpace(folderName);

            await ExecuteWithSessionAsync(
                async sessionId =>
                {
                    var parameters = new Dictionary<string, string>
                    {
                        ["api"] = "SYNO.FileStation.CreateFolder",
                        ["version"] = _options.FileStationVersion.ToString(),
                        ["method"] = "create",
                        ["folder_path"] = parentFolder,
                        ["name"] = folderName,
                        ["force_parent"] = "true",
                        ["_sid"] = sessionId
                    };

                    await PostFormAsync<JsonElement>(
                        "entry.cgi",
                        parameters,
                        cancellationToken);

                    return true;
                },
                cancellationToken);
        }

        public async Task<string> UploadAsync(
            string folderPath,
            FileUpload file,
            bool overwrite = true,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Length is <= 0)
            {
                throw new ArgumentException("File is empty.", nameof(file));
            }

            return await ExecuteWithSessionAsync(
                async sessionId =>
                {
                    using var formData = new MultipartFormDataContent();

                    AddStringPart(formData, "api", "SYNO.FileStation.Upload");
                    AddStringPart(formData, "version", _options.FileStationVersion.ToString());
                    AddStringPart(formData, "method", "upload");
                    AddStringPart(formData, "path", folderPath);
                    AddStringPart(formData, "overwrite", overwrite ? "true" : "false");

                    using var fileContent = new StreamContent(file.Content);
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                        string.IsNullOrWhiteSpace(file.ContentType)
                            ? "application/octet-stream"
                            : file.ContentType);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "\"file\"",
                        FileName = $"\"{Path.GetFileName(file.FileName)}\"",
                        FileNameStar = Path.GetFileName(file.FileName)
                    };

                    formData.Add(fileContent);

                    using var response = await _httpClient.PostAsync(
                        BuildEndpointWithSid("entry.cgi", sessionId),
                        formData,
                        cancellationToken);

                    var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

                    EnsureHttpSuccess(response, responseText);

                    var apiResponse = DeserializeSynologyResponse<JsonElement>(responseText);
                    EnsureSynologySuccess(apiResponse, responseText);

                    return CombineSynologyPath(folderPath, Path.GetFileName(file.FileName));
                },
                cancellationToken);
        }

        public async Task<Stream> DownloadAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var memoryStream = new MemoryStream();

            try
            {
                await DownloadToStreamAsync(
                    filePath,
                    memoryStream,
                    cancellationToken);

                memoryStream.Position = 0;
                return memoryStream;
            }
            catch
            {
                await memoryStream.DisposeAsync();
                throw;
            }
        }

        public async Task DownloadToStreamAsync(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            if (destination is null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            await ExecuteWithSessionAsync(
                async sessionId =>
                {
                    var parameters = new Dictionary<string, string>
                    {
                        ["api"] = "SYNO.FileStation.Download",
                        ["version"] = _options.FileStationVersion.ToString(),
                        ["method"] = "download",
                        ["path"] = filePath,
                        ["_sid"] = sessionId
                    };

                    using var request = new HttpRequestMessage(HttpMethod.Post, "entry.cgi")
                    {
                        Content = new FormUrlEncodedContent(parameters)
                    };

                    using var response = await _httpClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                    EnsureHttpSuccess(response, null);

                    var mediaType = response.Content.Headers.ContentType?.MediaType;

                    if (string.Equals(mediaType, "application/json", StringComparison.OrdinalIgnoreCase))
                    {
                        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
                        var apiResponse = DeserializeSynologyResponse<JsonElement>(responseText);
                        EnsureSynologySuccess(apiResponse, responseText);

                        throw new SynologyException(
                            "Synology returned JSON instead of file content.",
                            responseText);
                    }

                    await response.Content.CopyToAsync(destination, cancellationToken);
                    return true;
                },
                cancellationToken);
        }

        private async Task<TResult> ExecuteWithSessionAsync<TResult>(
            Func<string, Task<TResult>> action,
            CancellationToken cancellationToken)
        {
            var sessionId = await LoginAsync(cancellationToken);

            try
            {
                return await action(sessionId);
            }
            finally
            {
                try
                {
                    await LogoutAsync(sessionId, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Synology logout failed.");
                }
            }
        }

        private async Task<string> LoginAsync(
            CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>
            {
                ["api"] = "SYNO.API.Auth",
                ["version"] = _options.AuthVersion.ToString(),
                ["method"] = "login",
                ["account"] = _options.Account,
                ["passwd"] = _options.Password,
                ["session"] = _options.Session,
                ["format"] = "sid"
            };

            var response = await PostFormAsync<SynologyLoginData>(
                "auth.cgi",
                parameters,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(response.Data?.Sid))
            {
                throw new SynologyException("Synology login succeeded but SID is empty.");
            }

            return response.Data.Sid;
        }

        private async Task LogoutAsync(
            string sessionId,
            CancellationToken cancellationToken)
        {
            var parameters = new Dictionary<string, string>
            {
                ["api"] = "SYNO.API.Auth",
                ["version"] = _options.AuthVersion.ToString(),
                ["method"] = "logout",
                ["session"] = _options.Session,
                ["_sid"] = sessionId
            };

            await PostFormAsync<JsonElement>(
                "auth.cgi",
                parameters,
                cancellationToken);
        }

        private async Task<SynologyApiResponse<T>> PostFormAsync<T>(
            string endpoint,
            Dictionary<string, string> parameters,
            CancellationToken cancellationToken)
        {
            using var content = new FormUrlEncodedContent(parameters);

            using var response = await _httpClient.PostAsync(
                endpoint,
                content,
                cancellationToken);

            var responseText = await response.Content.ReadAsStringAsync(cancellationToken);

            EnsureHttpSuccess(response, responseText);

            var apiResponse = DeserializeSynologyResponse<T>(responseText);
            EnsureSynologySuccess(apiResponse, responseText);

            return apiResponse;
        }

        private static SynologyApiResponse<T> DeserializeSynologyResponse<T>(
            string responseText)
        {
            try
            {
                var apiResponse = JsonSerializer.Deserialize<SynologyApiResponse<T>>(
                    responseText,
                    JsonOptions);

                return apiResponse
                    ?? throw new SynologyException("Synology response is empty.", responseText);
            }
            catch (JsonException ex)
            {
                throw new SynologyException(
                    $"Invalid Synology JSON response. {ex.Message}",
                    responseText);
            }
        }

        private static void EnsureHttpSuccess(
            HttpResponseMessage response,
            string? responseText)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            throw new SynologyException(
                $"Synology HTTP request failed. StatusCode: {(int)response.StatusCode} {response.StatusCode}.",
                responseText);
        }

        private void EnsureSynologySuccess<T>(
            SynologyApiResponse<T> apiResponse,
            string responseText)
        {
            if (apiResponse.Success)
            {
                return;
            }

            var code = apiResponse.Error?.Code;

            _logger.LogWarning(
                "Synology API failed. ErrorCode: {ErrorCode}. Response: {Response}",
                code,
                responseText);

            throw new SynologyException(
                $"Synology API failed. ErrorCode: {code?.ToString() ?? "unknown"}.",
                responseText,
                code);
        }

        private static void AddStringPart(
            MultipartFormDataContent formData,
            string name,
            string value)
        {
            var content = new StringContent(value, Encoding.UTF8);
            content.Headers.ContentType = null;
            formData.Add(content, name);
        }

        private static string BuildEndpointWithSid(
            string endpoint,
            string sessionId)
        {
            return $"{endpoint}?_sid={Uri.EscapeDataString(sessionId)}";
        }

        private static string CombineSynologyPath(
            string folderPath,
            string fileName)
        {
            return $"{folderPath.TrimEnd('/')}/{fileName}";
        }

        private sealed record SynologyApiResponse<T>
        {
            [JsonPropertyName("success")]
            public bool Success { get; init; }

            [JsonPropertyName("data")]
            public T? Data { get; init; }

            [JsonPropertyName("error")]
            public SynologyApiError? Error { get; init; }
        }

        private sealed record SynologyApiError
        {
            [JsonPropertyName("code")]
            public int Code { get; init; }
        }

        private sealed record SynologyLoginData
        {
            [JsonPropertyName("sid")]
            public string? Sid { get; init; }
        }
    }
}
