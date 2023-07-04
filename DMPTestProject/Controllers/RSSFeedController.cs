using DMPTestProject.Data;
using DMPTestProject.Models.RSS_Feed;
using DMPTestProject.Models.RSS_Feed.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            List<RSSFeed> rssFeeds = await dmpDbContext.Feeds.ToListAsync();
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
            var feed = new RSSFeed
            {
                Url = addRSSFeedViewModel.Url,
                Name = addRSSFeedViewModel.Name
            };

            await dmpDbContext.Feeds.AddAsync(feed);
            await dmpDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            return View();
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

        // GET: RSSFeedController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RSSFeedController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
