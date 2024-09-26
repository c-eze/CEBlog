using CEBlog.Data;
using CEBlog.Models;
using CEBlog.ViewModels; 
using Microsoft.AspNetCore.Mvc;

namespace CEBlog.ViewComponents
{
    public class TickerViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TickerViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = _context.Posts
                        .OrderByDescending(p => p.Created)
                        .Take(3)
                        .ToList();

            List<TickerViewModel> tickerVM = new List<TickerViewModel>();

            foreach (Post post in posts) 
            {
                tickerVM.Add(new TickerViewModel()
                {
                    Title = post.Title,
                    Slug = post.Slug
                });
            }

            return View("Default", tickerVM);
        }
    }
}
