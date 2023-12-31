﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMPTestProject.Models.RSS_Feed
{
    public class Content
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Author { get; set; }
        [DisplayFormat(DataFormatString = "{dd. MM. yyyy HH:mm:ss}")]
        public DateTime? PubDate { get; set; }
        public string? Url { get; set; }
        public RSSFeed Feed { get; set; } = new RSSFeed();

    }
}
