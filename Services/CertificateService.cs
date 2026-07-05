using JotronCertificateApp.Data;
using JotronCertificateApp.Dtos;
using JotronCertificateApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JotronCertificateApp.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _context;
    private readonly IFileService _fileService;

    public CertificateService(AppDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Certificate> CreateFromDtoAsync(CertificateUploadDto dto, string filePath)
    {
        var cert = new Certificate {
            CertificateNumber = dto.CertificateNumber,
            CertificateType = dto.CertificateType,
            NotifiedBody = dto.NotifiedBody,
            DateOfIssue = dto.DateOfIssue,
            ExpiryDate = dto.ExpiryDate,
            FilePath = filePath
        };

        var rev = new Revision {
            Rev = dto.Rev,
            IssueDate = dto.RevisionIssueDate,
            Reason = dto.Reason,
            Author = dto.Author,
            Approval = dto.Approval,
            Certificate = cert
        };

        _context.Certificates.Add(cert);
        _context.Revisions.Add(rev);
        await _context.SaveChangesAsync();

        return cert;
    }

    public async Task<IEnumerable<Certificate>> GetAllAsync(string? number, string? type)
    {
        var query = _context.Certificates.AsQueryable();
        if (!string.IsNullOrEmpty(number)) query = query.Where(c => c.CertificateNumber.Contains(number));
        if (!string.IsNullOrEmpty(type)) query = query.Where(c => c.CertificateType.Contains(type));
        return await query.Include(c => c.Revisions).ToListAsync();
    }

    public async Task<Certificate?> GetByIdAsync(int id) => 
        await _context.Certificates.Include(c => c.Revisions).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> DeleteAsync(int id)
    {
        var cert = await _context.Certificates.FindAsync(id);
        if (cert == null) return false;

        var filePath = cert.FilePath;
        _context.Certificates.Remove(cert);
        var deleted = await _context.SaveChangesAsync() > 0;

        if (deleted && !string.IsNullOrWhiteSpace(filePath))
        {
            try
            {
                await _fileService.DeleteFileAsync(filePath);
            }
            catch
            {
                // Ignore cleanup failures; deleting the DB record is primary.
            }
        }

        return deleted;
    }
}