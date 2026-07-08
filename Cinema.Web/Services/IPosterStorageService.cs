namespace Cinema.Web.Services
{
    /// <summary>
    /// Abstraction for poster image storage.
    /// Implementations can use local filesystem (dev) or MinIO/S3 (production).
    /// </summary>
    public interface IPosterStorageService
    {
        /// <summary>
        /// Upload a poster image and return the stored file name.
        /// </summary>
        Task<string> UploadAsync(IFormFile file, CancellationToken ct = default);

        /// <summary>
        /// Delete a poster image by file name.
        /// </summary>
        Task DeleteAsync(string fileName, CancellationToken ct = default);

        /// <summary>
        /// Get the public URL for a poster image.
        /// Returns a relative path (local) or full URL (MinIO).
        /// </summary>
        string GetUrl(string fileName);
    }
}
