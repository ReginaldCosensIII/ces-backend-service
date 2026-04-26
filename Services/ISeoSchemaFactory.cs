using System.Collections.Generic;
using CES.BackendService.Models;

namespace CES.BackendService.Services;

public interface ISeoSchemaFactory
{
    string GenerateFaqSchema(IEnumerable<Faq> faqs);
    string GenerateTechTipSchema(IEnumerable<TechTip> techTips);
}
