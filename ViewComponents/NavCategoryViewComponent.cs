using CEBlog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CEBlog.ViewComponents
{
    public class NavCategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public NavCategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var blogs = _context.Blogs;

            return View("Default", await blogs.ToListAsync());
        }
    }
}
