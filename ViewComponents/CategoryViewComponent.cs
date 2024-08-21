using CEBlog.Data;
using CEBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEBlog.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    { 
        private readonly ApplicationDbContext _context;

        public CategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = _context.Blogs.Include(b => b.Posts);

            return View("Default", await blogs.ToListAsync());
        }

    }
}
