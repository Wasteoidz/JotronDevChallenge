namespace JotronCertificateApp.Dtos;

public record CertificateResponseDto(
    int Id,
    string CertificateNumber,
    string CertificateType,
    string NotifiedBody,
    DateTime DateOfIssue,
    DateTime ExpiryDate,
    IEnumerable<RevisionDto> Revisions)
{
    public static CertificateResponseDto From(Models.Certificate certificate) => new(
        certificate.Id,
        certificate.CertificateNumber,
        certificate.CertificateType,
        certificate.NotifiedBody,
        certificate.DateOfIssue,
        certificate.ExpiryDate,
        certificate.Revisions.Select(r => new RevisionDto(r.Rev, r.IssueDate, r.Reason, r.Author, r.Approval)));
}
