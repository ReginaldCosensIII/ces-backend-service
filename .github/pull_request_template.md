# Pull Request: fix/routing-normalization

## 📋 Summary
Normalized backend routing logic to eliminate the hardcoded `/api` prefix, enabling seamless hosting as an IIS sub-application.

## 🎯 Objective
To fix the "double-api" path issue (`/api/api/seo/faqs`) occurring when the application is hosted in IIS under the `/api` virtual directory, while preserving the correct URL structure in local development.

## ✅ Scope Included
- [x] Implemented environment-aware Path Base injection in `Program.cs` (`app.UsePathBase("/api")`).
- [x] Stripped the hardcoded `/api` prefix from endpoints in `SeoEndpoints.cs`.
- [x] Audited `ForumEndpoints.cs` to ensure clean routing definitions.

## ⏳ Scope Intentionally Deferred
- [ ] Any broader configuration changes not related to IIS path routing.

## 🛠️ Implementation & Technical Notes
Used `app.UsePathBase` specifically for `IsDevelopment()` to mimic the IIS `/api` virtual directory without polluting the application's actual endpoint route strings. This ensures `app.MapGet("/seo/faqs")` consistently binds, and middleware correctly trims the `/api` segment locally.

## 📂 Areas Changed
- **Endpoints:** `SeoEndpoints.cs` (normalized routes)
- **Config:** `Program.cs` (added local PathBase logic)

## 🧪 Manual Verification Completed (Pre-Production IIS)
- [x] Build completed successfully (`dotnet build`)
- [x] App launched successfully
- [ ] Core feature behavior tested (e.g., Browser output verified)
- [ ] SEO/Schema validation (Google Rich Results Test)
- [ ] Error handling/Logging reviewed

## ⚠️ Blockers, Assumptions, or Risks
Assuming local development always occurs using the built-in Kestrel server or IIS Express which doesn't natively map the application to an `/api` virtual directory without configuration.

## 📝 Documentation & Follow-up
- [x] README.md updated
- [x] ROADMAP.md updated
- [ ] Follow-up Task: Validate endpoints in the production-lite server environment.

## 🏁 Done Criteria Check
The routing paths are decoupled from the hosting path, fulfilling the requirement for environment-agnostic deployment while matching the IIS architecture.