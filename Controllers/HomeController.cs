using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CEBlog.Data;
using CEBlog.Models;
using CEBlog.Services;
using CEBlog.ViewModels;
using X.PagedList.Extensions;
using CEBlog.Enums;

namespace CEBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
		private readonly BlogSearchService _blogSearchService;

        public HomeController(ILogger<HomeController> logger,
                              IBlogEmailSender emailSender,
                              ApplicationDbContext context,
                              BlogSearchService blogSearchService)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _blogSearchService = blogSearchService;
        }

        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 7;

            //var posts = _context.Posts
            //	.Include(p => p.Blog)
            //	.Include(p => p.Comments)
            //	.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
            //	.OrderByDescending(p => p.Created)
            //	.ToPagedList(pageNumber, pageSize);

            var posts = _blogSearchService.GetPosts()
                .OrderByDescending(p => p.Created);

            return View(posts.ToPagedList(pageNumber, pageSize));
		}

		public IActionResult About()
		{
			return View();
		}

		public IActionResult Contact()
		{
			return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            //This is where we will be emailing...
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.FirstName, model.LastName, model.Message);
            return RedirectToAction("Index");
        }

        public IActionResult Subscribe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([Bind("Email")] SubscribeTo model)
        {
            //This is where we will be emailing...
            await _emailSender.SendSubscribeEmailAsync(model.Email);

            string subject = "Thanks for signing up!";
            string message = $"<b>Thanks for signing up!</b><br/><p>Welcome! You are now subscribed to Chikere.dev blog. We will be passing along updates to the blog and much more.</p><br/><p>The Chikere.dev Team</p>";
            await _emailSender.SendEmailAsync(model.Email, subject, message);
            return RedirectToAction("Index");
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
