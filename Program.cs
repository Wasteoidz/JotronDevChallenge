using System.Text.Json.Serialization;
using FluentValidation;
using JotronCertificateApp.Data;
using JotronCertificateApp.Dtos;
using JotronCertificateApp.Endpoints;
using JotronCertificateApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddValidatorsFromAssemblyContaining<CertificateUploadDto>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(db, builder.Environment.ContentRootPath);
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapCertificateEndpoints();

app.Run();