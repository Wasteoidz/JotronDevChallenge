namespace JotronCertificateApp.Models;

public class Revision
{
    public int Id { get; set; }
    public string Rev { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Approval { get; set; } = string.Empty;

    // Relasjoner
    public int CertificateId { get; set; } // Foreign Key
    public Certificate Certificate { get; set; } = null!; // Navigation property
}