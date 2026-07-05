using Microsoft.EntityFrameworkCore;
using FluentValidation;
using JotronCertificateApp.Data;
using JotronCertificateApp.Endpoints;
using JotronCertificateApp.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. Infratstruktur og Datatilgang
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

// 2. Applikasjonstjenester (Dependency Injection)
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();

// 3. Valideringslag (Automatisk skanning av assembly)
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// 4. Middleware Pipeline
app.UseDefaultFiles();
app.UseStaticFiles();

// 5. Miljøbetinget Database Seeding (Isolert runtime-initialisering)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    
    DbInitializer.Seed(context, env.WebRootPath);
}

// 6. API Endepunkter (Modularisert ruting)
app.MapCertificateEndpoints();

app.Run();