using Microsoft.EntityFrameworkCore;
using JotronCertificateApp.Models;

namespace JotronCertificateApp.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context, string basePath)
    {
        // Garanterer skjemaintegritet
        context.Database.Migrate();

        if (context.Certificates.Any()) return;

        // Dedikert lagringsmappe i samsvar med arkitektur-feedback
        var storagePath = Path.Combine(basePath, "uploads");
        Directory.CreateDirectory(storagePath);

        var mockCerts = new List<Certificate>
        {
            new() {
                CertificateNumber = "JOT-2026-001",
                CertificateType = "Declaration of conformity",
                NotifiedBody = "DNV GL",
                DateOfIssue = new DateTime(2026, 1, 15),
                ExpiryDate = new DateTime(2031, 1, 15),
                FilePath = Path.Combine(storagePath, "mock1.pdf"),
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
                FilePath = Path.Combine(storagePath, "mock2.pdf"),
                Revisions = new List<Revision> {
                    new() { Rev = "01", IssueDate = new DateTime(2025, 6, 20), Reason = "Initial Inventory", Author = "AN", Approval = "BV Approved" }
                }
            }
        };

        // DB-transaksjon sikrer atomisitet mellom DB og filsystem
        using var transaction = context.Database.BeginTransaction();
        try
        {
            context.Certificates.AddRange(mockCerts);
            context.SaveChanges();

            // I/O kjøres kun hvis DB-insert var vellykket
            foreach (var cert in mockCerts)
            {
                if (!File.Exists(cert.FilePath))
                {
                    File.WriteAllText(cert.FilePath, $"Fiktivt PDF-innhold for sertifikat {cert.CertificateNumber}");
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}