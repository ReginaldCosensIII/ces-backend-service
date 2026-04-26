using Microsoft.EntityFrameworkCore;
using CES.BackendService.Contracts;
using CES.BackendService.Data;
using CES.BackendService.Endpoints;
using CES.BackendService.Options;
using CES.BackendService.Services;
using CES.BackendService.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) => config
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("C:/logs/ces-backend/log-.txt", rollingInterval: RollingInterval.Day));

// 1. Add Services
builder.Services.AddOpenApi();

// Register SEO Factory
builder.Services.AddSingleton<ISeoSchemaFactory, SeoSchemaFactory>();

// Register Memory Cache for SEO Schema caching
builder.Services.AddMemoryCache();

// Register Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register SMTP Options
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.SectionName));

// Register Email Service
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

// Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ForumRegistrationRequestValidator>();

// 2. Add Rate Limiting (5 requests per 10 minutes per IP)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ForumRegistrationPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(10);
        opt.QueueLimit = 0;
    });

    // Strategy based on IP address
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Detail = "Too many registration attempts. Please try again in 10 minutes."
        }, token);
    };
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        var path = exceptionHandlerPathFeature?.Path;

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred while processing request path: {RequestPath}", path);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        
        var detail = app.Environment.IsDevelopment()
            ? exception?.ToString()
            : "An error occurred while processing your registration. Our team has been notified.";

        await context.Response.WriteAsJsonAsync(new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Detail = detail
        });
    });
});

// 3. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Use Rate Limiting
app.UseRateLimiter();

// Setup Path Base for local dev to match IIS routing
if (app.Environment.IsDevelopment())
{
    app.UsePathBase("/api");
}

// 4. Map Endpoints
// Note: Internal mapping to /ceo-ai-forum/register
// IIS Sub-application at /api will make this effectively /api/ceo-ai-forum/register
app.MapForumEndpoints();
app.MapSeoEndpoints();

app.Run();
