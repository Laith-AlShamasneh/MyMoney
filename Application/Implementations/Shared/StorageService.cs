using Application.DTOs.Shared;
using Application.Interfaces.Shared;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Application.Implementations.Shared;

public class StorageService(IConfiguration configuration, ILogger<StorageService> logger) : IStorageService
{
    private readonly string _rootPath = configuration["FileStorage:RootPath"]
                    ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    private readonly ILogger<StorageService> _logger = logger;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    private const int LargeFileBufferSize = 1 * 1024 * 1024; // 1 MB
    private const int SmallFileBufferSize = 80 * 1024;       // 80 KB
    private const long LargeFileThreshold = 10L * 1024 * 1024; // 10 MB

    #region Save Operations

    public async Task<FileSaveResult> SaveFileAsync(
        string relativeFolder,
        IFormFile file,
        FileUploadType fileType,
        string? customFileName = null,
        bool createUniqueFolder = false,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(file, nameof(file));

        // Use the Stream overload to avoid code duplication
        using var stream = file.OpenReadStream();
        var fileName = !string.IsNullOrWhiteSpace(customFileName)
            ? customFileName + Path.GetExtension(file.FileName)
            : file.FileName;

        return await SaveFileInternalAsync(relativeFolder, stream, fileName, fileType, createUniqueFolder, cancellationToken);
    }

    public async Task<FileSaveResult> SaveFileStreamAsync(
        string relativeFolder,
        Stream stream,
        string fileName,
        FileUploadType fileType,
        bool createUniqueFolder = false,
        CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(stream, nameof(stream));
        return await SaveFileInternalAsync(relativeFolder, stream, fileName, fileType, createUniqueFolder, cancellationToken);
    }

    private async Task<FileSaveResult> SaveFileInternalAsync(
        string relativeFolder,
        Stream inputStream,
        string originalFileName,
        FileUploadType fileType,
        bool createUniqueFolder,
        CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(relativeFolder, nameof(relativeFolder));

        try
        {
            // 1. Validate Extension
            var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
            ValidateExtension(extension, fileType);

            // 2. Prepare Folder Path
            var targetFolder = NormalizeRelativePath(relativeFolder);
            if (createUniqueFolder)
            {
                targetFolder = Path.Combine(targetFolder, Guid.NewGuid().ToString());
            }

            var fullFolderPath = Path.Combine(_rootPath, targetFolder);
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            // 3. Generate Secure Filename
            var finalFileName = string.IsNullOrWhiteSpace(originalFileName)
                ? $"{Guid.NewGuid():N}{extension}"
                : $"{Path.GetFileNameWithoutExtension(originalFileName)}-{Guid.NewGuid():N}{extension}";

            var fullFilePath = Path.Combine(fullFolderPath, finalFileName);

            int bufferSize = inputStream.Length > LargeFileThreshold ? LargeFileBufferSize : SmallFileBufferSize;

            // 4. Write to Disk
            await using (var fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
            {
                if (inputStream.CanSeek) inputStream.Position = 0;
                await inputStream.CopyToAsync(fileStream, cancellationToken);
            }

            // 5. Return Result
            _contentTypeProvider.TryGetContentType(finalFileName, out var contentType);

            // Return path with forward slashes for URL compatibility
            var savedRelativePath = Path.Combine(targetFolder, finalFileName).Replace('\\', '/');

            return new FileSaveResult(
                FolderName: Path.GetFileName(targetFolder),
                FileName: finalFileName,
                RelativePath: savedRelativePath,
                FullPath: fullFilePath,
                Extension: extension,
                ContentType: contentType,
                Size: new FileInfo(fullFilePath).Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName} to {Folder}", originalFileName, relativeFolder);
            throw;
        }
    }

    #endregion

    #region Read Operations

    public async Task<FileInfoResult?> GetFileInfoAsync(
        string relativeFolderPath,
        string fileName,
        string? baseUrl = null,
        CancellationToken cancellationToken = default)
    {
        var fullPath = await FindFileAsync(relativeFolderPath, fileName, cancellationToken);
        if (fullPath == null) return null;

        var info = new FileInfo(fullPath);
        var relPath = Path.Combine(relativeFolderPath, info.Name).Replace('\\', '/');

        return new FileInfoResult(
            FileName: info.Name,
            RelativePath: relPath,
            FullPath: fullPath,
            Size: info.Length,
            Extension: info.Extension,
            Url: GenerateUrl(relPath, baseUrl));
    }

    public async Task<FileGetResult?> GetFileStreamAsync(
        string relativeFolderPath,
        string fileName,
        string? baseUrl = null,
        CancellationToken cancellationToken = default)
    {
        var fullPath = await FindFileAsync(relativeFolderPath, fileName, cancellationToken);
        if (fullPath == null) return null;

        var info = new FileInfo(fullPath);
        _contentTypeProvider.TryGetContentType(info.Name, out var contentType);

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, SmallFileBufferSize, true);
        var relPath = Path.Combine(relativeFolderPath, info.Name).Replace('\\', '/');

        return new FileGetResult(
            FileName: info.Name,
            FullPath: fullPath,
            Stream: stream, // Caller must dispose
            ContentType: contentType,
            Size: info.Length,
            Extension: info.Extension,
            Url: GenerateUrl(relPath, baseUrl));
    }

    public Task<IList<FileInfoResult>> GetFilesFromFolderAsync(
        string relativeFolderPath,
        string? baseUrl = null,
        CancellationToken cancellationToken = default)
    {
        var fullFolderPath = Path.Combine(_rootPath, NormalizeRelativePath(relativeFolderPath));

        if (!Directory.Exists(fullFolderPath))
            return Task.FromResult<IList<FileInfoResult>>([]);

        var results = Directory.GetFiles(fullFolderPath)
            .Select(f => new FileInfo(f))
            .Select(fi =>
            {
                var rel = Path.Combine(relativeFolderPath, fi.Name).Replace('\\', '/');
                return new FileInfoResult(
                    FileName: fi.Name,
                    RelativePath: rel,
                    FullPath: fi.FullName,
                    Size: fi.Length,
                    Extension: fi.Extension,
                    Url: GenerateUrl(rel, baseUrl));
            })
            .ToList();

        return Task.FromResult<IList<FileInfoResult>>(results);
    }

    #endregion

    #region Delete Operations

    public async Task<bool> DeleteFileAsync(string relativeFolderPath, string fileName)
    {
        var fullPath = await FindFileAsync(relativeFolderPath, fileName);
        if (fullPath == null) return false;

        try
        {
            File.Delete(fullPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {Path}", fullPath);
            return false;
        }
    }

    public Task<bool> DeleteFolderAsync(string relativeFolderPath)
    {
        var fullPath = Path.Combine(_rootPath, NormalizeRelativePath(relativeFolderPath));
        if (!Directory.Exists(fullPath)) return Task.FromResult(false);

        try
        {
            Directory.Delete(fullPath, true);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete folder {Path}", fullPath);
            return Task.FromResult(false);
        }
    }

    #endregion

    #region Helpers

    private async Task<string?> FindFileAsync(string relativeFolder, string fileName, CancellationToken ct = default)
    {
        var folder = Path.Combine(_rootPath, NormalizeRelativePath(relativeFolder));
        if (!Directory.Exists(folder)) return null;

        var fullPath = Path.Combine(folder, fileName);
        if (File.Exists(fullPath)) return fullPath;

        // Fallback: Check without case sensitivity if strictly needed, or handle partial names
        // For performance, direct path check is preferred.
        return await Task.FromResult<string?>(null);
    }

    private static string NormalizeRelativePath(string path)
    {
        return path.Replace('/', Path.DirectorySeparatorChar)
                   .Replace('\\', Path.DirectorySeparatorChar)
                   .Trim(Path.DirectorySeparatorChar);
    }

    private static string? GenerateUrl(string relativePath, string? baseUrl)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) return null;
        return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    private static void ValidateExtension(string extension, FileUploadType type)
    {
        string[] allowed = type switch
        {
            FileUploadType.UserProfileImage => [".jpg", ".jpeg", ".png", ".webp"],
            FileUploadType.GroupCoverImage => [".jpg", ".jpeg", ".png", ".webp"],
            FileUploadType.PaymentReceipt => [".jpg", ".jpeg", ".png", ".pdf"],
            FileUploadType.Document => [".pdf", ".doc", ".docx", ".xls", ".xlsx"],
            _ => []
        };

        if (allowed.Length > 0 && !allowed.Contains(extension))
        {
            throw new InvalidOperationException($"Extension '{extension}' is not allowed for {type}. Allowed: {string.Join(",", allowed)}");
        }
    }

    /// <summary>
    /// Minimal ContentType Provider to avoid external dependencies.
    /// In production, consider using 'Microsoft.AspNetCore.StaticFiles'.
    /// </summary>
    private sealed class FileExtensionContentTypeProvider
    {
        private readonly ConcurrentDictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase);

        public FileExtensionContentTypeProvider()
        {
            // Images
            _map[".png"] = "image/png";
            _map[".jpg"] = "image/jpeg";
            _map[".jpeg"] = "image/jpeg";
            _map[".webp"] = "image/webp";
            // Docs
            _map[".pdf"] = "application/pdf";
            _map[".doc"] = "application/msword";
            _map[".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            // Text
            _map[".txt"] = "text/plain";
            _map[".json"] = "application/json";
        }

        public bool TryGetContentType(string fileName, out string contentType)
        {
            var ext = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(ext) && _map.TryGetValue(ext, out var ct))
            {
                contentType = ct;
                return true;
            }
            contentType = "application/octet-stream";
            return false;
        }
    }

    #endregion
}