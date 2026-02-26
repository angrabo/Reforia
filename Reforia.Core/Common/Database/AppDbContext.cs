using Microsoft.EntityFrameworkCore;
using Reforia.Core.Common.Config;

namespace Reforia.Core.Common.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<ConfigItem> Config { get; set; }
}