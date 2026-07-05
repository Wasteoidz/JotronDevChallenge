using JotronCertificateApp.Dtos; 
using JotronCertificateApp.Models;

namespace JotronCertificateApp.Services;

public interface ICertificateService
{
    Task<Certificate> CreateFromDtoAsync(CertificateUploadDto dto, string filePath);
    Task<Revision> AddRevisionFromDtoAsync(int certificateId, RevisionDto dto);
    Task<IEnumerable<Certificate>> GetAllAsync(string? number, string? type);
    Task<Certificate?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}