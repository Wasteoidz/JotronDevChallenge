namespace JotronCertificateApp.Models;

public class Certificate
{
    public int Id { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public string NotifiedBody { get; set; } = string.Empty;
    public DateTime DateOfIssue { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public ICollection<Revision> Revisions { get; set; } = new List<Revision>();
}