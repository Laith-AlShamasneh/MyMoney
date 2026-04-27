namespace Domain.Shared;

/// <summary>
/// Central registry for file storage paths.
/// Ensures all file uploads go to consistent, designated directories.
/// </summary>
public static class StoragePaths
{
    private static readonly Dictionary<FileUploadType, string> _paths = new()
    {
        { FileUploadType.UserProfileImage, Path.Combine("assets", "images", "users", "profile") },
        { FileUploadType.Document,         Path.Combine("assets", "documents", "general") }
    };

    public static string GetPath(FileUploadType type)
    {
        return _paths.TryGetValue(type, out var path)
            ? path
            : Path.Combine("general", "others");
    }
}