using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CEBlog.Data;
using CEBlog.Models;
using CEBlog.Services;
using CEBlog.ViewModels;
using X.PagedList.Extensions;
using CEBlog.Enums;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

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
                
        public IActionResult GetContact(ContactMe contactMe)
        {
            JsonResponseViewModel model = new JsonResponseViewModel();
            if (contactMe.Email is not null && contactMe.Message is not null)
            {
                model.ResponseCode = 0;

                //This is where we will be emailing... 
                _emailSender.SendContactEmailAsync(contactMe.Email, contactMe.Message);
            }
            else
            {
                model.ResponseCode = 1;
                model.ResponseMessage = "No email available";
            }
            return Json(model); 
        }

        public IActionResult GetSubscriber(SubscribeTo subscriber)
        {
			JsonResponseViewModel model = new JsonResponseViewModel();
			if (subscriber.Email is not null)
			{
				model.ResponseCode = 0; 

				_emailSender.SendSubscribeEmailAsync(subscriber.Email);

				string subject = "Thanks for signing up!";
				string message = $"<b>Thanks for signing up!</b><br/><p>Welcome! You are now subscribed to Chikere.dev blog. We will be passing along updates to the blog and much more.</p><br/><p>The Chikere.dev Team</p>";
				_emailSender.SendEmailAsync(subscriber.Email, subject, message);
			}
			else
			{
				model.ResponseCode = 1;
				model.ResponseMessage = "No email available";
			}
			return Json(model);
		}

        public IActionResult Subscribe()
        {
            return View();
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

        [Route("/StatusCodeError/{statusCode}")]
		public IActionResult Error(int statusCode)
		{
			return View();
		}
	}
}
