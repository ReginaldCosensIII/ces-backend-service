using CES.BackendService.Contracts;

namespace CES.BackendService.Services;

public interface IEmailService
{
    Task SendRegistrationEmailAsync(SeminarRegistrationRequest request);
}
