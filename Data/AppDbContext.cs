using Microsoft.EntityFrameworkCore;
using JotronCertificateApp.Models;
namespace JotronCertificateApp.Data;
public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Revision> Revisions { get; set; }
}