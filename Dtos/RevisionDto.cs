namespace JotronCertificateApp.Dtos;

public record RevisionDto(
    string Rev, 
    DateTime IssueDate, 
    string Reason, 
    string Author, 
    string Approval);