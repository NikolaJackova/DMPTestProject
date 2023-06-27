using System.ComponentModel.DataAnnotations.Schema;

namespace DMPTestProject.Models.RSS_Feed
{
    public class RSSFeed
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }

        public ICollection<Content> Contents { get; set; }
    }
}
