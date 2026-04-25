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

namespace CES.BackendService.Endpoints;

public static class SeoEndpoints
{
    public static void MapSeoEndpoints(this WebApplication app)
    {
        app.MapGet("/api/seo/faqs", async (
            [FromServices] AppDbContext dbContext,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ISeoSchemaFactory seoSchemaFactory) =>
        {
            if (!memoryCache.TryGetValue("seo_faqs", out string? schemaString))
            {
                var faqs = await dbContext.Faqs.Where(f => f.IsPublished).ToListAsync();
                schemaString = seoSchemaFactory.GenerateFaqSchema(faqs);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                memoryCache.Set("seo_faqs", schemaString, cacheOptions);
            }

            return Results.Content(schemaString!, "application/ld+json");
        });

        app.MapGet("/api/seo/techtips", async (
            [FromServices] AppDbContext dbContext,
            [FromServices] IMemoryCache memoryCache,
            [FromServices] ISeoSchemaFactory seoSchemaFactory) =>
        {
            if (!memoryCache.TryGetValue("seo_techtips", out string? schemaString))
            {
                var techTips = await dbContext.TechTips.Where(t => t.IsPublished).ToListAsync();
                schemaString = seoSchemaFactory.GenerateTechTipSchema(techTips);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                memoryCache.Set("seo_techtips", schemaString, cacheOptions);
            }

            return Results.Content(schemaString!, "application/ld+json");
        });
    }
}