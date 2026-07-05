namespace JotronCertificateApp.Dtos;

public record CertificateUploadDto
{
    public string CertificateNumber { get; init; } = string.Empty;
    public string CertificateType { get; init; } = string.Empty;
    public string NotifiedBody { get; init; } = string.Empty;
    public DateTime DateOfIssue { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Rev { get; init; } = string.Empty;
    public DateTime RevisionIssueDate { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Approval { get; init; } = string.Empty;
    public IFormFile? File { get; init; }
}