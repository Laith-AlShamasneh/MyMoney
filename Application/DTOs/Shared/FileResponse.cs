namespace Application.DTOs.Shared;

/// <summary>
/// Result returned after saving a file.
/// </summary>
public sealed record FileSaveResult(
    string FolderName,
    string FileName,
    string RelativePath,
    string FullPath,
    string Extension,
    string ContentType,
    long Size);

/// <summary>
/// Result returned when opening a file for reading with URL. 
/// NOTE: Caller is responsible for disposing the Stream.
/// </summary>
public sealed record FileGetResult(
    string FileName,
    string FullPath,
    Stream Stream,
    string ContentType,
    long Size,
    string Extension,
    string? Url);

/// <summary>
/// Lightweight metadata about files inside a folder (no Stream).
/// </summary>
public sealed record FileInfoResult(
    string FileName,
    string RelativePath,
    string FullPath,
    long Size,
    string Extension,
    string? Url);