using FluentValidation;
using System.IO;

namespace JotronCertificateApp.Dtos.Validation;
public class CertificateValidator : AbstractValidator<CertificateUploadDto>
{
    private static readonly string[] AllowedExtensions = { ".pdf", ".png", ".jpg", ".jpeg" };
    private static readonly string[] AllowedContentTypes = {
        "application/pdf",
        "image/png",
        "image/jpeg"
    };

    public CertificateValidator()
    {
        RuleFor(x => x.CertificateNumber).NotEmpty();
        RuleFor(x => x.CertificateType).NotEmpty();
        RuleFor(x => x.NotifiedBody).NotEmpty();
        RuleFor(x => x.DateOfIssue).NotEmpty();
        RuleFor(x => x.ExpiryDate).GreaterThan(x => x.DateOfIssue);
        RuleFor(x => x.Rev).NotEmpty();
        RuleFor(x => x.RevisionIssueDate).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
        RuleFor(x => x.Author).NotEmpty();
        RuleFor(x => x.Approval).NotEmpty();
        RuleFor(x => x.File).NotNull()
            .Must(file => file != null && file.Length <= 5 * 1024 * 1024)
                .WithMessage("The file must be 5MB or smaller.")
            .Must(file => file != null && AllowedExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
                .WithMessage("Only PDF and image files are allowed.")
            .Must(file => file != null && AllowedContentTypes.Contains(file.ContentType?.ToLowerInvariant()))
                .WithMessage("Only PDF and image files are allowed.");
    }
}