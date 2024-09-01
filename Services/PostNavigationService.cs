using CEBlog.Data;
using Microsoft.EntityFrameworkCore;

namespace CEBlog.Services
{
    public class PostNavigationService : INavigationService
    {
        private readonly ApplicationDbContext _context;

        public PostNavigationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Next(int id)
        {
            int position = id;
            var posts = _context.Posts
                            .ToList();

            position++;
            if (position > posts.Count())
            {
                position = 1;
            }

            return posts.FirstOrDefault(p => p.Id == position).Slug;
        }

        public string Previous(int id)
        {
            int position = id;
            var posts = _context.Posts
                            .ToList();

            position--;
            if (position < 1)
            {
                position = posts.Count();
            }

            return posts.FirstOrDefault(p => p.Id == position).Slug;
        }
    }
}
