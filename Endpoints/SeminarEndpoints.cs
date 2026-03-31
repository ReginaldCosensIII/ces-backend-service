using CES.BackendService.Contracts;
using CES.BackendService.Services;
using FluentValidation;

namespace CES.BackendService.Endpoints;

public static class SeminarEndpoints
{
    public static void MapSeminarEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/seminar/register", async (
            SeminarRegistrationRequest request,
            IValidator<SeminarRegistrationRequest> validator,
            IEmailService emailService) =>
        {
            // 1. Validation
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // 2. Honeypot check (Silent drop)
            if (!string.IsNullOrWhiteSpace(request.Honeypot))
            {
                return Results.Ok(new { message = "Registration received." });
            }

            // 3. Send Email
            await emailService.SendRegistrationEmailAsync(request);

            // 4. Success Response
            return Results.Ok(new { message = "Registration received." });
        })
        .WithName("RegisterSeminar")
        .RequireRateLimiting("SeminarRegistrationPolicy")
        .WithOpenApi();
    }
}
