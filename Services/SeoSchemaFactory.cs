using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CES.BackendService.Models;

namespace CES.BackendService.Services;

public class SeoSchemaFactory : ISeoSchemaFactory
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true // Makes it easier to read in the browser
    };

    public string GenerateFaqSchema(IEnumerable<Faq> faqs)
    {
        var publishedFaqs = faqs.Where(f => f.IsPublished == true);

        // Using Dictionary to force the literal '@' character into the JSON key
        var schema = new Dictionary<string, object>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "FAQPage",
            ["mainEntity"] = publishedFaqs.Select(faq => new Dictionary<string, object>
            {
                ["@type"] = "Question",
                ["name"] = faq.Question,
                ["acceptedAnswer"] = new Dictionary<string, object>
                {
                    ["@type"] = "Answer",
                    ["text"] = faq.Answer
                }
            }).ToArray()
        };

        return JsonSerializer.Serialize(schema, _jsonOptions);
    }

    public string GenerateTechTipSchema(IEnumerable<TechTip> techTips)
    {
        var publishedTips = techTips.Where(t => t.IsPublished == true);

        var articles = publishedTips.Select(tip => new Dictionary<string, object>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "TechArticle",
            ["headline"] = tip.Title,
            ["text"] = tip.Content,
            ["datePublished"] = tip.CreatedAt.ToString("O"),
            ["dateModified"] = (tip.UpdatedAt ?? tip.CreatedAt).ToString("O"),
            ["image"] = new Dictionary<string, object>
            {
                ["@type"] = "ImageObject",
                ["url"] = "https://www.cesitservice.com/images/socials/tech-tips-social.png"
            },
            ["author"] = new Dictionary<string, object>
            {
                ["@type"] = "Organization",
                ["name"] = "Computer Enhancement Systems, Inc."
            }
        }).ToArray();

        var schema = new Dictionary<string, object>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "ItemList",
            ["itemListElement"] = articles.Select((article, index) => new Dictionary<string, object>
            {
                ["@type"] = "ListItem",
                ["position"] = index + 1,
                ["item"] = article
            }).ToArray()
        };

        return JsonSerializer.Serialize(schema, _jsonOptions);
    }
}