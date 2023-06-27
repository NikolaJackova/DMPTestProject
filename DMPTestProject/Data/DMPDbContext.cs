using DMPTestProject.Models.RSS_Feed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DMPTestProject.Data
{
    public class DMPDbContext : DbContext
    {
        public DMPDbContext(DbContextOptions<DMPDbContext> options) : base(options) { }

        public DbSet<RSSFeed> Feeds { get; set; }
        public DbSet<Content> Contents { get; set; }
    }
}
