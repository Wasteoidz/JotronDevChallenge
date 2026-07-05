using FluentValidation;
using JotronCertificateApp.Dtos;
using JotronCertificateApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace JotronCertificateApp.Endpoints;

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/certificates");

        group.MapGet("/", async (string? number, string? type, ICertificateService cs) => 
            Results.Ok(await cs.GetAllAsync(number, type)));

        group.MapPost("/", async (
            [FromForm] CertificateUploadDto dto, 
            [FromServices] IValidator<CertificateUploadDto> validator,
            IFileService fs, ICertificateService cs) =>
        {
            var res = await validator.ValidateAsync(dto);
            if (!res.IsValid) return Results.ValidationProblem(res.ToDictionary());

            if (dto.File is null)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["File"] = new[] { "A file is required." }
                });
            }

            var path = await fs.SaveFileAsync(dto.File);
            var cert = await cs.CreateFromDtoAsync(dto, path);
            return Results.Created($"/certificates/{cert.Id}", cert);
        }).DisableAntiforgery();

        group.MapGet("/{id:int}/download", async (int id, ICertificateService cs) =>
        {
            var cert = await cs.GetByIdAsync(id);
            return cert != null && File.Exists(cert.FilePath) 
                ? Results.File(cert.FilePath, "application/pdf") 
                : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, ICertificateService cs) =>
        {
            var deleted = await cs.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}