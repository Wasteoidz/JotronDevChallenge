namespace JotronCertificateApp.Services;

public class FileService : IFileService
{
    private readonly string _uploadsFolder;

    public FileService(IWebHostEnvironment env)
        => _uploadsFolder = Path.Combine(env.ContentRootPath, "uploads");

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        Directory.CreateDirectory(_uploadsFolder);
        var path = Path.Combine(_uploadsFolder, $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);
        return path;
    }

    public Task DeleteFileAsync(string path)
    {
        if (File.Exists(path)) File.Delete(path);
        return Task.CompletedTask;
    }
}