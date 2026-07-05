namespace JotronCertificateApp.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
}