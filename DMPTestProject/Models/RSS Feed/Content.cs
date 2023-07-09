using System.ComponentModel.DataAnnotations.Schema;

namespace DMPTestProject.Models.RSS_Feed
{
    public class Content
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Author { get; set; }
        public DateTime? PubDate { get; set; }
        public string Url { get; set; }
        public RSSFeed Feed { get; set; } = new RSSFeed();

    }
}
