namespace JotronCertificateApp.Models;

public record CertificateUploadModel(
    string CertificateNumber,
    string CertificateType,
    string NotifiedBody,
    DateTime DateOfIssue,
    DateTime ExpiryDate,
    IFormFile File
);