namespace DMPTestProject.Models.RSS_Feed
{
    public class Content
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
        public RSSFeed Feed { get; set; }

    }
}
