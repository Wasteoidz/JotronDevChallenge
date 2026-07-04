using Microsoft.EntityFrameworkCore;
using JotronCertificateApp.Data;
using JotronCertificateApp.Models;

namespace JotronCertificateApp.Services;

public interface ICertificateService
{
    Task<Certificate> CreateAsync(Certificate cert, Revision initialRevision);
    Task<List<Certificate>> GetAllAsync(string? number, string? type);
    Task<Certificate?> GetByIdAsync(int id);
    Task<Revision> AddRevisionAsync(int certificateId, Revision revision);
    Task<bool> DeleteAsync(int id);
}

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _db;
    public CertificateService(AppDbContext db) => _db = db;

    public async Task<Certificate> CreateAsync(Certificate cert, Revision initialRevision)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try {
            cert.Revisions.Add(initialRevision);
            _db.Certificates.Add(cert);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return cert;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Certificate>> GetAllAsync(string? number, string? type)
    {
        var query = _db.Certificates.Include(c => c.Revisions).AsQueryable();

        if (!string.IsNullOrWhiteSpace(number))
            query = query.Where(c => c.CertificateNumber.Contains(number));

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(c => c.CertificateType == type);

        return await query.ToListAsync();
    }

    public async Task<Certificate?> GetByIdAsync(int id) => 
        await _db.Certificates.Include(c => c.Revisions).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Revision> AddRevisionAsync(int certificateId, Revision revision)
    {
        var cert = await _db.Certificates.Include(c => c.Revisions)
            .FirstOrDefaultAsync(c => c.Id == certificateId);
            
        if (cert == null) throw new KeyNotFoundException("Sertifikat ikke funnet.");

        cert.Revisions.Add(revision);
        await _db.SaveChangesAsync();
        return revision;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try {
            var cert = await _db.Certificates.FindAsync(id);
            if (cert == null) return false;

            _db.Certificates.Remove(cert);
            await _db.SaveChangesAsync();

            if (File.Exists(cert.FilePath)) {
                File.Delete(cert.FilePath);
            }

            await transaction.CommitAsync();
            return true;
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }
}