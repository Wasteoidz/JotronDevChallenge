using Microsoft.AspNetCore.Http;

namespace JotronCertificateApp.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
}