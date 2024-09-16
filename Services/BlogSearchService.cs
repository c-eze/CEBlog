using CEBlog.Data;
using CEBlog.Enums;
using CEBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace CEBlog.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;

        public BlogSearchService(ApplicationDbContext context,      
                                 IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        public List<Post> ArchiveSearch(string? publishDate)
        {
            var posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Comments)
                                      .Include(p => p.Tags)
									  .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
									  .OrderByDescending(p => p.Created)
									  .ToList();

			var newPosts = new List<Post>();

			if (!string.IsNullOrEmpty(publishDate))
            {
                foreach (Post post in posts)
                {
                    string createdDate = post.Created.ToString("MMM.dd.yyyy");
                    if (createdDate.Contains(publishDate))
                    {
                        newPosts.Add(post);
                    }
                }         
                return newPosts;
            }
            return posts;
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

        public IQueryable<Post> GetPosts()
        {
            IQueryable<Post> posts;

            posts = _memoryCache.Get<IQueryable<Post>>("posts");

            if(posts is null)
            {
                posts = _context.Posts.Include(p => p.Blog)
                                      .Include(p => p.Comments)
                                      .Include(p => p.Tags)
                                      .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
									  .AsQueryable();

                _memoryCache.Set("employees", posts, TimeSpan.FromMinutes(1));
            }

            return posts;
        }
    }
}
