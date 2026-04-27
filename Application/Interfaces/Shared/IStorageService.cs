using Application.DTOs.Shared;
using Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Shared;

public interface IStorageService
{
    // === Write Operations ===

    /// <summary>
    /// Saves a file uploaded via HTTP Form (IFormFile).
    /// </summary>
    Task<FileSaveResult> SaveFileAsync(
        string relativeFolder,
        IFormFile file,
        FileUploadType fileType,
        string? customFileName = null,
        bool createUniqueFolder = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a file from a generic Stream (useful for generated reports/receipts).
    /// </summary>
    Task<FileSaveResult> SaveFileStreamAsync(
        string relativeFolder,
        Stream stream,
        string fileName,
        FileUploadType fileType,
        bool createUniqueFolder = false,
        CancellationToken cancellationToken = default);

    // === Read Operations ===

    Task<FileInfoResult?> GetFileInfoAsync(
        string relativeFolderPath,
        string fileName,
        string? baseUrl = null,
        CancellationToken cancellationToken = default);

    Task<FileGetResult?> GetFileStreamAsync(
        string relativeFolderPath,
        string fileName,
        string? baseUrl = null,
        CancellationToken cancellationToken = default);

    Task<IList<FileInfoResult>> GetFilesFromFolderAsync(
        string relativeFolderPath,
        string? baseUrl = null,
        CancellationToken cancellationToken = default);

    // === Delete Operations ===

    Task<bool> DeleteFileAsync(string relativeFolderPath, string fileName);
    Task<bool> DeleteFolderAsync(string relativeFolderPath);
}