namespace Cinema.Web.Services
{
    /// <summary>
    /// Local filesystem implementation of poster storage.
    /// Used in development when MinIO is not available.
    /// </summary>
    public class LocalPosterStorageService : IPosterStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalPosterStorageService> _logger;

        public LocalPosterStorageService(IWebHostEnvironment env, ILogger<LocalPosterStorageService> logger)
        {
            _env = env;
            _logger = logger;
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken ct = default)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "images", "phim");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            _logger.LogInformation("Poster uploaded locally: {FileName}", fileName);
            return fileName;
        }

        public Task DeleteAsync(string fileName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName == "no-image.jpg")
                return Task.CompletedTask;

            var filePath = Path.Combine(_env.WebRootPath, "images", "phim", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Poster deleted locally: {FileName}", fileName);
            }

            return Task.CompletedTask;
        }

        public string GetUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "/images/phim/no-image.jpg";

            if (fileName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return fileName;

            return $"/images/phim/{fileName}";
        }
    }
}

