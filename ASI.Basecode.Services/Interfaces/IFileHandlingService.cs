using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IFileHandlingService
    {
        Task<string> HandleFile(IFormFile file, string folderPath);
    }
}