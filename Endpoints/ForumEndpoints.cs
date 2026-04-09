using CES.BackendService.Contracts;
using CES.BackendService.Services;
using FluentValidation;

namespace CES.BackendService.Endpoints;

public static class ForumEndpoints
{
    public static void MapForumEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/ceo-ai-forum/register", async (
            ForumRegistrationRequest request,
            IValidator<ForumRegistrationRequest> validator,
            IEmailService emailService) =>
        {
            // 1. Honeypot check (Silent drop — runs first to prevent bot fingerprinting)
            if (!string.IsNullOrWhiteSpace(request.Honeypot))
            {
                return Results.Ok(new { message = "Registration received." });
            }

            // 2. Validation
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            // 3. Send Email
            await emailService.SendRegistrationEmailAsync(request);

            // 4. Success Response
            return Results.Ok(new { message = "Registration received." });
        })
        .WithName("RegisterForum")
        .RequireRateLimiting("ForumRegistrationPolicy")
        .WithOpenApi();
    }
}
