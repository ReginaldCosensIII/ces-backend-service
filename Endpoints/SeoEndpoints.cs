using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using CES.BackendService.Data;
using CES.BackendService.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CES.BackendService.Endpoints;

public static class SeoEndpoints
{
    public static void MapSeoEndpoints(this WebApplication app)
    {
        app.MapGet("/seo/faqs", async (
            [FromServices] AppDbContext dbContext,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ISeoSchemaFactory seoSchemaFactory,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                if (!memoryCache.TryGetValue("seo_faqs", out string? schemaString))
                {
                    var faqs = await dbContext.Faqs.Where(f => f.IsPublished).OrderBy(f => f.DisplayOrder).ToListAsync();
                    schemaString = seoSchemaFactory.GenerateFaqSchema(faqs);

                    memoryCache.Set("seo_faqs", schemaString);
                }

                return Results.Content(schemaString!, "application/ld+json");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching {EndpointEntity}", "Faqs");
                return Results.Problem(detail: "An unexpected error occurred processing your request.", statusCode: 500);
            }
        });

        app.MapGet("/seo/techtips", async (
            [FromServices] AppDbContext dbContext,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ISeoSchemaFactory seoSchemaFactory,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                if (!memoryCache.TryGetValue("seo_techtips", out string? schemaString))
                {
                    var techTips = await dbContext.TechTips.Where(t => t.IsPublished).OrderBy(t => t.DisplayOrder).ToListAsync();
                    schemaString = seoSchemaFactory.GenerateTechTipSchema(techTips);

                    memoryCache.Set("seo_techtips", schemaString);
                }

                return Results.Content(schemaString!, "application/ld+json");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching {EndpointEntity}", "TechTips");
                return Results.Problem(detail: "An unexpected error occurred processing your request.", statusCode: 500);
            }
        });

        app.MapPost("/seo/flush-cache", (
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                memoryCache.Remove("seo_faqs");
                memoryCache.Remove("seo_techtips");
                return Results.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while flushing the SEO cache");
                return Results.Problem(detail: "An unexpected error occurred processing your request.", statusCode: 500);
            }
        });
    }
}