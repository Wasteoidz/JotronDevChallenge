using JotronCertificateApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JotronCertificateApp.Data;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Revision> Revisions => Set<Revision>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>()
            .HasMany(c => c.Revisions)
            .WithOne(r => r.Certificate)
            .HasForeignKey(r => r.CertificateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}