using CEBlog.Data;
using CEBlog.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEBlog.ViewComponents
{
    public class RecentPostViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public RecentPostViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = _context.Posts
                .Include(b => b.Blog)
                .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .Take(2);

            return View("Default", await blogs.ToListAsync());

        }
    }
}
