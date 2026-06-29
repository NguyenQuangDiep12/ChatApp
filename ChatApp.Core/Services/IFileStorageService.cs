using System.IO;
using System.Threading.Tasks;

namespace ChatApp.Core.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folder);
        Task DeleteFileAsync(string fileUrl);
    }
}
