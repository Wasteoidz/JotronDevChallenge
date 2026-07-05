using JotronCertificateApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JotronCertificateApp.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context, string rootPath)
    {
        context.Database.Migrate();

        string uploadsPath = Path.Combine(rootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);

        foreach (var fileName in new[] { "mock1.pdf", "mock2.pdf" })
        {
            string filePath = Path.Combine(uploadsPath, fileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, $"Mock content for {fileName}");
            }
        }

        if (context.Certificates.Any()) return;

        var mockCerts = new List<Certificate>
        {
            new() {
                CertificateNumber = "JOT-2026-001",
                CertificateType = "Declaration of conformity",
                NotifiedBody = "DNV GL",
                DateOfIssue = new DateTime(2026, 1, 15),
                ExpiryDate = new DateTime(2031, 1, 15),
                FilePath = Path.Combine(uploadsPath, "mock1.pdf"),
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
                FilePath = Path.Combine(uploadsPath, "mock2.pdf"),
                Revisions = new List<Revision> {
                    new() { Rev = "01", IssueDate = new DateTime(2025, 6, 20), Reason = "Initial Inventory", Author = "AN", Approval = "BV Approved" }
                }
            }
        };

        context.Certificates.AddRange(mockCerts);
        context.SaveChanges();
    }
}