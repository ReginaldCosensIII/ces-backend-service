# CES Backend Service (.NET 10 Minimal API)

## Architecture
This project is architected as a lightweight, high-performance web service tailored for local deployment as an IIS sub-application.
- **Minimal APIs**: Providing low overhead HTTP routing without MVC controllers.
- **FluentValidation**: Enforcing strict server-side validation rules.
- **Serilog**: Capturing structured, dynamically rolling log files for advanced diagnostics.

## Configuration
This service handles sensitive credentials for email dispatch.

### User Secrets (Local Development)
Ensure secrets are not written to `appsettings.json`. Use the .NET Secret Manager instead:
```bash
dotnet user-secrets init
dotnet user-secrets set "SmtpOptions:Host" "<YOUR_SMTP_HOST>"
dotnet user-secrets set "SmtpOptions:Port" "587"
dotnet user-secrets set "SmtpOptions:Username" "<YOUR_SMTP_USERNAME>"
dotnet user-secrets set "SmtpOptions:Password" "<YOUR_SMTP_PASSWORD>"
dotnet user-secrets set "SmtpOptions:FromAddress" "noreply@cesitservice.com"
dotnet user-secrets set "SmtpOptions:ToAddress" "admin@cesitservice.com"
```

### Environment Variables (Production)
For production deployments inside IIS, bind the keys securely via Environment Variables utilizing double-underscores (`__`) to declare the hierarchy:
- `SmtpOptions__Host`
- `SmtpOptions__Port`
- `SmtpOptions__Username`
- `SmtpOptions__Password`
- `SmtpOptions__FromAddress`
- `SmtpOptions__ToAddress`

## Security
To protect the endpoints from spam and brute-force form submissions, the application implements a strict **Fixed-Window Rate Limiting** policy natively connected to the endpoints:
- **Rate**: 5 requests
- **Window**: Every 10 minutes (per IP)
- Requests exceeding this threshold receive an immediate HTTP 429 response accompanied by a structured `ProblemDetails` message.
