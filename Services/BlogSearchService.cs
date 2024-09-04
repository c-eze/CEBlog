using CEBlog.Data;
using CEBlog.Enums;
using CEBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CEBlog.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;

        public BlogSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> AuthorSearch(string authorId)
        {
            var posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Author)
                                      .Include(p => p.Comments)
                                      .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();

            if (!string.IsNullOrEmpty(authorId))
            {
                posts = posts.Where(p => p.Author.Id == authorId);
            }

            return posts.OrderByDescending(p => p.Created);
        }

        public IQueryable<Post> TagSearch(string tagName)
        {
            var posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Comments)
                                      .Include(p => p.Tags)
                                      .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();

            if (tagName != null)
            {
                tagName = tagName.ToLower();

                posts = posts.Where
                    (
                        p => p.Tags.Any(t => t.Text.ToLower() == tagName)
                    );
            }

            return posts.OrderByDescending(p => p.Created);
        }

        public IQueryable<Post> CategorySearch(string categoryName)
        {
            var posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Comments)
                                      .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();

            if (categoryName != null)
            {
                categoryName = categoryName.ToLower();

                posts = posts.Where(
                    p => p.Blog.Name.ToLower().Contains(categoryName));
            }

            return posts.OrderByDescending(p => p.Created);
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            var posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Comments)
                                      .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();

            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();

                posts = posts.Where(
                    p => p.Title.ToLower().Contains(searchTerm) ||
                    p.Abstract.ToLower().Contains(searchTerm) ||
                    p.Content.ToLower().Contains(searchTerm) ||
                    p.Comments.Any(c => c.Body.ToLower().Contains(searchTerm) ||
                                        c.ModeratedBody.ToLower().Contains(searchTerm) ||
                                        c.Author.FirstName.ToLower().Contains(searchTerm) ||
                                        c.Author.LastName.ToLower().Contains(searchTerm) ||
                                        c.Author.Email.ToLower().Contains(searchTerm)));
            }

            return posts.OrderByDescending(p => p.Created);
        }
    }
}
