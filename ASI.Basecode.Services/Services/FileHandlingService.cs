using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASI.Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ASI.Basecode.Services.Services
{
    public class FileHandlingService : IFileHandlingService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileHandlingService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<string> HandleFile(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
            {
                throw new InvalidOperationException("File is null or empty");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, and WEBP are allowed.");
            }

            if (file.Length > _maxFileSize)
            {
                throw new InvalidOperationException("File size exceeds the maximum limit of 5 MB.");
            }

            var fileName = $"product_{Guid.NewGuid()}{fileExtension}";
            
            var imagePath = await _fileStorageService.UploadImageAsync(file, folderPath, fileName);
            return imagePath;
        }
    }
}