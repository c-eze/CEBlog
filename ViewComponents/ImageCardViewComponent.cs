using CEBlog.Data;
using CEBlog.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace CEBlog.ViewComponents
{
    public class ImageCardViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ImageCardViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(Post imagePost)
        { 
            return View("Default", imagePost);
        }
    }
}
