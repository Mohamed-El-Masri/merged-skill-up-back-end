using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkillUpPlatform.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SkillUpPlatform.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;
    private readonly string _uploadPath;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _uploadPath = _configuration["FileStorage:UploadPath"] ?? "uploads";
        
        // Create upload directory if it doesn't exist
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string fileType)
    {
        try
        {
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, fileContent);
            _logger.LogInformation($"File saved successfully: {filePath}");
            
            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to save file: {fileName}");
            throw;
        }
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return await File.ReadAllBytesAsync(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get file: {filePath}");
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation($"File deleted successfully: {filePath}");
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to delete file: {filePath}");
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            return File.Exists(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error checking file existence: {filePath}");
            return false;
        }
    }

    public async Task<long> GetFileSizeAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_uploadPath, filePath);
            if (File.Exists(fullPath))
            {
                var fileInfo = new FileInfo(fullPath);
                return fileInfo.Length;
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting file size: {filePath}");
            return 0;
        }
    }

    public async Task<string> GetFileUrlAsync(string filePath)
    {
        var baseUrl = _configuration["FileStorage:BaseUrl"] ?? "https://localhost:5001";
        return $"{baseUrl}/files/{filePath}";
    }

    public bool IsValidFileType(string fileType)
    {
        var allowedTypesSection = _configuration.GetSection("FileStorage:AllowedTypes");
        var allowedTypes = allowedTypesSection.GetChildren().Select(x => x.Value).ToArray();
        
        if (!allowedTypes.Any())
        {
            allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf", "text/plain" };
        }
        
        return allowedTypes.Contains(fileType.ToLower());
    }

    public bool IsValidFileSize(long fileSize)
    {
        var maxSizeInBytes = long.Parse(_configuration["FileStorage:MaxFileSizeInBytes"] ?? "10485760"); // 10MB default
        return fileSize <= maxSizeInBytes;
    }

    public string GenerateFileHash(byte[] fileContent)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(fileContent);
        return Convert.ToBase64String(hash);
    }
}
