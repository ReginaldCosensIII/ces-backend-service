using Microsoft.EntityFrameworkCore;
using CES.BackendService.Models;

namespace CES.BackendService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Faq> Faqs { get; set; }
    public DbSet<TechTip> TechTips { get; set; }
}
