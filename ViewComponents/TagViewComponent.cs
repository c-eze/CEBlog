using CEBlog.Data;
using CEBlog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Data.Entity;

namespace CEBlog.ViewComponents
{
    public class TagViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TagViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
		{
            var tags = _context.Tags
                        .Select(t => t.Text.ToLower())
                        .ToList();

			return View("Default", tags);
        }
    }
}
