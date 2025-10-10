using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(IFormFile file, string containerName, string fileName);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}