using System.Threading.RateLimiting;
using CES.BackendService.Contracts;
using CES.BackendService.Endpoints;
using CES.BackendService.Options;
using CES.BackendService.Services;
using CES.BackendService.Validation;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services
builder.Services.AddOpenApi();

// Register SMTP Options
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.SectionName));

// Register Email Service
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<SeminarRegistrationRequestValidator>();

// 2. Add Rate Limiting (3 requests per 10 minutes per IP)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("SeminarRegistrationPolicy", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(10);
        opt.QueueLimit = 0;
    });

    // Strategy based on IP address
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// 3. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use Rate Limiting
app.UseRateLimiter();

// 4. Map Endpoints
// Note: Internal mapping to /seminar/register
// IIS Sub-application at /api will make this effectively /api/seminar/register
app.MapSeminarEndpoints();

// Existing Weather Forecast (for testing)
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireRateLimiting("SeminarRegistrationPolicy"); // Just for testing

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
