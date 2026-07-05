using JotronCertificateApp.Data;
using JotronCertificateApp.Models;
using JotronCertificateApp.Dtos;
using Microsoft.EntityFrameworkCore;

namespace JotronCertificateApp.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _context;

    public CertificateService(AppDbContext context) => _context = context;

    public async Task<Certificate> CreateFromDtoAsync(CertificateUploadDto dto, string filePath)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
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
            await transaction.CommitAsync();

            return cert;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Revision> AddRevisionFromDtoAsync(int certificateId, RevisionDto dto)
    {
        var cert = await _context.Certificates.FindAsync(certificateId) ?? throw new KeyNotFoundException();
        
        var rev = new Revision {
            Rev = dto.Rev,
            IssueDate = dto.IssueDate,
            Reason = dto.Reason,
            Author = dto.Author,
            Approval = dto.Approval,
            CertificateId = certificateId
        };

        _context.Revisions.Add(rev);
        await _context.SaveChangesAsync();
        return rev;
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
        _context.Certificates.Remove(cert);
        return await _context.SaveChangesAsync() > 0;
    }
}