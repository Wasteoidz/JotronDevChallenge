using Microsoft.AspNetCore.Http;
using System.IO;

namespace JotronCertificateApp.Services;

public class FileService : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        // 1. Valider filendelse (Whitelist)
        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Ugyldig filtype. Kun PDF og bilder er tillatt.");
        }

        // 2. Valider maksstørrelse (F.eks. 5MB)
        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            throw new InvalidOperationException("Filen overstiger maksgrensen på 5MB.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Sanitiser filnavnet for å forhindre Path Traversal-angrep
        var trustedFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, trustedFileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }
}