using Minio;
using Minio.DataModel.Args;

namespace Cinema.Web.Services
{
    /// <summary>
    /// MinIO S3-compatible implementation of poster storage.
    /// Used in production for shared, scalable file storage.
    /// </summary>
    public class MinioPosterStorageService : IPosterStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioPosterStorageService> _logger;
        private readonly string _bucketName;
        private readonly string _publicBaseUrl;

        public MinioPosterStorageService(
            IMinioClient minioClient,
            IConfiguration configuration,
            ILogger<MinioPosterStorageService> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
            _bucketName = configuration["MinIO:BucketName"] ?? "cinema-posters";
            _publicBaseUrl = configuration["MinIO:PublicBaseUrl"] ?? "/posters";
        }

        public async Task<string> UploadAsync(IFormFile file, CancellationToken ct = default)
        {
            await EnsureBucketExistsAsync(ct);

            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            using var stream = file.OpenReadStream();
            var putArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType ?? "image/jpeg");

            await _minioClient.PutObjectAsync(putArgs, ct);

            _logger.LogInformation("Poster uploaded to MinIO: {FileName} ({Size} bytes)",
                fileName, file.Length);

            return fileName;
        }

        public async Task DeleteAsync(string fileName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName == "no-image.jpg")
                return;

            try
            {
                var removeArgs = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(fileName);

                await _minioClient.RemoveObjectAsync(removeArgs, ct);
                _logger.LogInformation("Poster deleted from MinIO: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete poster from MinIO: {FileName}", fileName);
            }
        }

        public string GetUrl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return $"{_publicBaseUrl}/no-image.jpg";

            if (fileName.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return fileName;

            return $"{_publicBaseUrl}/{fileName}";
        }

        private async Task EnsureBucketExistsAsync(CancellationToken ct)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(_bucketName);
            if (!await _minioClient.BucketExistsAsync(existsArgs, ct))
            {
                var makeArgs = new MakeBucketArgs().WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(makeArgs, ct);
                _logger.LogInformation("Created MinIO bucket: {BucketName}", _bucketName);

                // Set bucket policy to public read for poster images
                var policyJson = $$"""
                {
                    "Version": "2012-10-17",
                    "Statement": [
                        {
                            "Effect": "Allow",
                            "Principal": {"AWS": ["*"]},
                            "Action": ["s3:GetObject"],
                            "Resource": ["arn:aws:s3:::{{_bucketName}}/*"]
                        }
                    ]
                }
                """;
                var policyArgs = new SetPolicyArgs()
                    .WithBucket(_bucketName)
                    .WithPolicy(policyJson);
                await _minioClient.SetPolicyAsync(policyArgs, ct);
                _logger.LogInformation("Set public read policy on bucket: {BucketName}", _bucketName);
            }
        }
    }
}

