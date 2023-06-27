namespace DMPTestProject.Models.RSS_Feed
{
    public class RSSFeed
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public ICollection<Content> Contents { get; set; }
    }
}
