using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using JotronCertificateApp.Models;
using JotronCertificateApp.Services;

namespace JotronCertificateApp.Endpoints;

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/certificates");

        group.MapPost("/", async ([FromForm] CertificateUploadDto dto, IFileService fs, ICertificateService cs, IValidator<CertificateUploadDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid) 
                return Results.ValidationProblem(validationResult.ToDictionary());

            if (dto.File == null) 
                return Results.BadRequest("Fil mangler.");

            var path = await fs.SaveFileAsync(dto.File);
            
            var cert = new Certificate {
                CertificateNumber = dto.CertificateNumber,
                CertificateType = dto.CertificateType,
                NotifiedBody = dto.NotifiedBody,
                DateOfIssue = dto.DateOfIssue,
                ExpiryDate = dto.ExpiryDate,
                FilePath = path
            };

            var rev = new Revision {
                Rev = dto.Rev,
                IssueDate = dto.RevisionIssueDate,
                Reason = dto.Reason,
                Author = dto.Author,
                Approval = dto.Approval
            };

            var result = await cs.CreateAsync(cert, rev);
            return Results.Created($"/certificates/{result.Id}", result);
        }).DisableAntiforgery();

        group.MapGet("/", async (string? number, string? type, ICertificateService cs) => 
            Results.Ok(await cs.GetAllAsync(number, type)));

        group.MapGet("/{id:int}/download", async (int id, ICertificateService cs) =>
        {
            var cert = await cs.GetByIdAsync(id);
            if (cert == null || !File.Exists(cert.FilePath)) 
                return Results.NotFound();
            
            return Results.File(cert.FilePath, "application/octet-stream", Path.GetFileName(cert.FilePath));
        });

        group.MapDelete("/{id:int}", async (int id, ICertificateService cs) =>
        {
            var deleted = await cs.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        group.MapPost("/{id:int}/revisions", async (int id, [FromForm] RevisionDto dto, ICertificateService cs) =>
        {
            var revision = new Revision {
                Rev = dto.Rev,
                IssueDate = dto.IssueDate,
                Reason = dto.Reason,
                Author = dto.Author,
                Approval = dto.Approval
            };

            try {
                var result = await cs.AddRevisionAsync(id, revision);
                return Results.Created($"/certificates/{id}", result);
            } catch (KeyNotFoundException) {
                return Results.NotFound();
            }
        }).DisableAntiforgery();
    }
}

public record RevisionDto(string Rev, DateTime IssueDate, string Reason, string Author, string Approval);