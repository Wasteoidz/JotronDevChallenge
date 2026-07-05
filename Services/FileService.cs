namespace JotronCertificateApp.Services;

public class FileService : IFileService
{
    private readonly string _uploadsFolder;

    public FileService(IWebHostEnvironment env) 
        => _uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file.Length > 5 * 1024 * 1024) throw new InvalidOperationException("Maks 5MB.");
        Directory.CreateDirectory(_uploadsFolder);
        var path = Path.Combine(_uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return path;
    }
}