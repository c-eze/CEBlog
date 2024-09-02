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

		public HomeController(ILogger<HomeController> logger,
                              IBlogEmailSender emailSender, 
                              ApplicationDbContext context)
		{
			_logger = logger;
			_emailSender = emailSender;
			_context = context;
		}

		public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 7;

			var posts = _context.Posts
				.Include(p => p.Blog)
				.Include(p => p.Comments)
				.Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
				.OrderByDescending(p => p.Created)
				.ToPagedList(pageNumber, pageSize);

			return View(posts);

			//var blogs = _context.Blogs.Where(
			//    b => b.Posts.Any(p => p.ReadyStatus == Enums.ReadyStatus.ProductionReady))
			//    .OrderByDescending(b => b.Created)
			//    .ToPagedList(pageNumber, pageSize);

			//var blogs = _context.Blogs
			//             .Include(b => b.Author)
			//          .OrderByDescending(b => b.Created)
			//          .ToPagedList(pageNumber, pageSize);

			//return View(blogs);
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
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
            return RedirectToAction("Index");
        }



		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
