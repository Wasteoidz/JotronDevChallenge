using FluentValidation;

namespace JotronCertificateApp.Dtos.Validation;
public class CertificateValidator : AbstractValidator<CertificateUploadDto>
{
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
        RuleFor(x => x.File).NotNull();
    }
}