using JotronCertificateApp.Models;

namespace JotronCertificateApp.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context, string webRootPath)
    {
        context.Database.EnsureCreated();

        if (context.Certificates.Any()) return;

        var mockCerts = new List<Certificate>
        {
            new() {
                CertificateNumber = "JOT-2026-001",
                CertificateType = "Declaration of conformity",
                NotifiedBody = "DNV GL",
                DateOfIssue = new DateTime(2026, 1, 15),
                ExpiryDate = new DateTime(2031, 1, 15),
                FilePath = Path.Combine(webRootPath, "mock1.pdf"),
                Revisions = new List<Revision> {
                    new() { Rev = "01", IssueDate = new DateTime(2026, 1, 15), Reason = "First version", Author = "GAJ", Approval = "Jotron QA" }
                }
            },
            new() {
                CertificateNumber = "JOT-2026-002",
                CertificateType = "Green passport",
                NotifiedBody = "Bureau Veritas",
                DateOfIssue = new DateTime(2025, 6, 20),
                ExpiryDate = new DateTime(2030, 6, 20),
                FilePath = Path.Combine(webRootPath, "mock2.pdf"),
                Revisions = new List<Revision> {
                    new() { Rev = "01", IssueDate = new DateTime(2025, 6, 20), Reason = "Initial Inventory", Author = "AN", Approval = "BV Approved" }
                }
            }
        };

        // Generer mock-filer kontrollert
        Directory.CreateDirectory(webRootPath);
        for (int i = 1; i <= mockCerts.Count; i++)
        {
            string filePath = Path.Combine(webRootPath, $"mock{i}.pdf");
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"Fiktivt PDF-innhold for sertifikat {i}");
            }
        }

        context.Certificates.AddRange(mockCerts);
        context.SaveChanges();
    }
}