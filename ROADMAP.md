# Project Roadmap

## Service Routing
- **IIS Optimization**: The backend service routing has been normalized to support seamless hosting as an IIS sub-application (`/api`). The application is environment-aware and dynamically uses a path base during local development to match the production URL structure without hardcoding the prefix in endpoint definitions.

## SEO Enhancements
- **Deferred Properties**: The `Author` and `Image` properties in the Schema.org JSON-LD generation (handled by `SeoSchemaFactory`) have been temporarily deferred. They will be integrated once the Tech Tip content strategy is finalized and the corresponding data fields become available in the models.
