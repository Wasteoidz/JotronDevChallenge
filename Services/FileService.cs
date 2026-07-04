using Microsoft.AspNetCore.Http;
using System.IO;

namespace JotronCertificateApp.Services;

public class FileService : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploads);
        var path = Path.Combine(uploads, $"{Guid.NewGuid()}_{file.FileName}");
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return path;
    }
}