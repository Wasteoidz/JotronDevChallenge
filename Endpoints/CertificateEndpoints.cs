using FluentValidation;
using JotronCertificateApp.Dtos;
using JotronCertificateApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace JotronCertificateApp.Endpoints;

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/certificates");

        group.MapGet("/", async (string? number, string? type, ICertificateService cs) =>
        {
            var certs = await cs.GetAllAsync(number, type);
            return Results.Ok(certs.Select(CertificateResponseDto.From));
        });

        group.MapGet("/{id:int}", async (int id, ICertificateService cs) =>
        {
            var cert = await cs.GetByIdAsync(id);
            return cert == null ? Results.NotFound() : Results.Ok(CertificateResponseDto.From(cert));
        });

        group.MapPost("/", async (
            [FromForm] CertificateUploadDto dto,
            [FromServices] IValidator<CertificateUploadDto> validator,
            IFileService fs, ICertificateService cs) =>
        {
            var res = await validator.ValidateAsync(dto);
            if (!res.IsValid) return Results.ValidationProblem(res.ToDictionary());

            var path = await fs.SaveFileAsync(dto.File!);
            var cert = await cs.CreateFromDtoAsync(dto, path);
            return Results.Created($"/certificates/{cert.Id}", CertificateResponseDto.From(cert));
        }).DisableAntiforgery();

        group.MapGet("/{id:int}/download", async (int id, ICertificateService cs) =>
        {
            var cert = await cs.GetByIdAsync(id);
            if (cert == null || !File.Exists(cert.FilePath)) return Results.NotFound();

            var provider = new FileExtensionContentTypeProvider();
            var contentType = provider.TryGetContentType(cert.FilePath, out var type)
                ? type
                : "application/octet-stream";

            var fileName = Path.GetFileName(cert.FilePath);
            return Results.File(cert.FilePath, contentType, fileName);
        });

        group.MapDelete("/{id:int}", async (int id, ICertificateService cs) =>
        {
            var deleted = await cs.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}