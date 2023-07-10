using DMPTestProject.Data;
using DMPTestProject.Models.RSS_Feed;
using DMPTestProject.Models.RSS_Feed.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;

namespace DMPTestProject.Controllers
{
    public class RSSFeedController : Controller
    {
        private readonly DMPDbContext dmpDbContext;
        public RSSFeedController(DMPDbContext dmpDbContext)
        {
            this.dmpDbContext = dmpDbContext;
        }

        // GET: RSSFeedController
        public async Task<IActionResult> Index()
        {
            List<RSSFeed> rssFeeds = await dmpDbContext.Feeds.OrderBy(feed => feed.Id).ToListAsync();
            return View(rssFeeds);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchName)
        {
            ViewData["SearchName"] = searchName;
            List<RSSFeed> rssFeeds = new List<RSSFeed>();
            if (searchName == string.Empty || searchName is null)
            {
                rssFeeds = await dmpDbContext.Feeds.OrderBy(feed => feed.Id).ToListAsync();
            }
            else
            {
                rssFeeds = await dmpDbContext.Feeds.Where(item => item.Name.ToLower().Contains(searchName.ToLower())).OrderBy(feed => feed.Id).ToListAsync();
            }
            return View(rssFeeds);
        }

        // GET: RSSFeedController/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: RSSFeedController/Add
        [HttpPost]
        public async Task<IActionResult> Add(AddRSSFeedViewModel addRSSFeedViewModel)
        {
            if (!IsValidFeedUrl(addRSSFeedViewModel.Url))
            {
                return BadRequest("Invalid feed URL");
            }
            var feed = new RSSFeed
            {
                Url = addRSSFeedViewModel.Url,
                Name = addRSSFeedViewModel.Name
            };
            await dmpDbContext.Feeds.AddAsync(feed);
            await dmpDbContext.SaveChangesAsync();
            try
            {
                RefreshFeed(feed.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet("RSSFeed/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            List<Content> content = dmpDbContext.Contents.Where(content => content.Feed.Id == id).OrderByDescending(content => content.PubDate).ToList();
            return View(content);
        }

        [HttpPost("RSSFeed/{id:int}")]
        public async Task<IActionResult> Detail(int id, DateTime dateFrom, DateTime dateTo)
        {
            if (dateTo == DateTime.MinValue)
            {
                dateTo = DateTime.MaxValue;
            }
            if (dateFrom != DateTime.MinValue)
            {
                ViewData["DateFrom"] = dateFrom.ToString("yyyy-MM-ddTHH:mm");
            }
            if (dateTo != DateTime.MaxValue)
            {
                ViewData["DateTo"] = dateTo.ToString("yyyy-MM-ddTHH:mm");
            }
            List<Content> content = dmpDbContext.Contents.Where(content => content.Feed.Id == id && content.PubDate <= dateTo.ToUniversalTime() && content.PubDate >= dateFrom.ToUniversalTime()).OrderByDescending(content => content.PubDate).ToList();
            return View(content);
        }

        // GET: RSSFeedController/Edit/5
        public ActionResult Edit(int id)
        {
            var feed = dmpDbContext.Feeds.Where(item => item.Id == id).FirstOrDefault();
            return View(feed);
        }

        // POST: RSSFeedController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                string url = collection["Url"];
                string name = collection["Name"];
                var feed = dmpDbContext.Feeds.Where(item => item.Id == id).FirstOrDefault();
                feed.Url = url;
                feed.Name = name;
                await dmpDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: RSSFeedController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                string[] ids = collection["ID"].ToString().Split(new char[] { ',' });
                foreach (string item in ids)
                {
                    if (item != string.Empty)
                    {
                        var feed = await dmpDbContext.Feeds.FindAsync(int.Parse(item));
                        dmpDbContext.Feeds.Remove(feed);
                        await dmpDbContext.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Refresh(int id)
        {
            try
            {
                RefreshFeed(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");
        }

        private void RefreshFeed(int id)
        {
            RSSFeed? feed = dmpDbContext.Feeds.Find(id);
            if (feed is not null)
            {
                Rss20FeedFormatter rssFormatter;

                using (var xmlReader = XmlReader.Create(feed.Url))
                {
                    rssFormatter = new Rss20FeedFormatter();
                    rssFormatter.ReadFrom(xmlReader);
                }

                foreach (var syndicationItem in rssFormatter.Feed.Items)
                {
                    if (!dmpDbContext.Contents.Any(content => syndicationItem.Title.Text == content.Title && content.Feed.Id == id))
                    {
                        Content content = new Content()
                        {
                            Author = syndicationItem.Authors.ElementAtOrDefault(0)?.Name,
                            Title = syndicationItem.Title.Text,
                            Description = syndicationItem.Summary.Text,
                            PubDate = syndicationItem.PublishDate.DateTime,
                            Url = syndicationItem.Links.ElementAtOrDefault(0)?.Uri.ToString(),
                            Feed = feed
                        };
                        dmpDbContext.Contents.Add(content);
                        feed.Contents.Add(content);
                        dmpDbContext.SaveChanges();
                    }
                }
            }
        }

        private bool IsValidFeedUrl(string url)
        {
            bool isValid = true;
            try
            {
                XmlReader reader = XmlReader.Create(url);
                Rss20FeedFormatter formatter = new Rss20FeedFormatter();
                formatter.ReadFrom(reader);
                reader.Close();
            }
            catch
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
