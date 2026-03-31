namespace CES.BackendService.Contracts;

public record SeminarRegistrationRequest(
    string FirstName,
    string LastName,
    string Title,
    string Company,
    string Email,
    string Phone,
    string Honeypot
);
