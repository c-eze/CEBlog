using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CEBlog.Data;
using CEBlog.Models;
using CEBlog.Services;
using CEBlog.Enums;
using CEBlog.ViewModels;
using X.PagedList.Extensions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using MailKit.Search;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CEBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(ApplicationDbContext context,
                               ISlugService slugService,
                               IImageService imageService,
                               UserManager<BlogUser> userManager,
                               BlogSearchService blogSearchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string? authorId)
        {
            //var applicationDbContext = _context.Posts.Include(p => p.Author).Include(p => p.Blog);
            var posts = from p in _context.Posts.Include(p => p.Author).Include(p => p.Blog)
                        select p;

			if (!string.IsNullOrEmpty(authorId))
            {
                posts = posts.Where(p => p.AuthorId == authorId);
            }

            return View(await posts.ToListAsync());
		}

        public IActionResult ArchiveIndex(int? page, string? publishDate)
        {          
            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.ArchiveSearch(publishDate);
            
            ViewData["PublishDate"] = publishDate;
            ViewData["TotalPosts"] = posts.Count();

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> AuthorIndex(int? page, string? authorId)
        {
            ViewData["AuthorDesc"] = "Meet Chikere, a developer in Alabama who specializes in programming full stack web applications and software in ASP.NET and C#.";

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.AuthorSearch(authorId);

            ViewData["TotalPosts"] = posts.Count();

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> TagIndex(int? page, string tagName)
        {
            ViewData["TagName"] = tagName;

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.TagSearch(tagName);

            ViewData["TotalPosts"] = posts.Count();

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> CategoryIndex(int? page, string categoryName)
        {
            ViewData["CategoryName"] = categoryName;

            var blog = _context.Blogs
                .Where(b => b.Name.Contains(categoryName))
                .FirstOrDefault();

            ViewData["Description"] = blog.Description.ToString();

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.CategorySearch(categoryName);

            ViewData["TotalPosts"] = posts.Count();

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public async Task<IActionResult> SearchIndex(int? page, string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var pageNumber = page ?? 1;
            var pageSize = 6;

            var posts = _blogSearchService.Search(searchTerm);

            ViewData["PostsFound"] = posts.Count();

            return View(posts.ToPagedList(pageNumber, pageSize));
        }

		//BlogPostIndex
		public async Task<IActionResult> BlogPostIndex(int? id, int? page)
		{
			if (id is null)
			{
				return NotFound();
			}

            var pageNumber = page ?? 1;
            var pageSize = 5;

            var posts = _context.Posts
                .Where(p => p.BlogId == id && p.ReadyStatus == ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .ToPagedList(pageNumber, pageSize);

			return View(posts);

		}

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            ViewData["Title"] = "Post Details";
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.RelatedPosts)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Moderator)
                .Include(p => p.Comments)
                .ThenInclude(c => c.Replies)
                .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            var relatedPostsIds = post.RelatedPosts.Select(r => r.ArticleId).ToList();
            var relatedPosts = new List<Post>();
            foreach (var postId in relatedPostsIds)
            {
                relatedPosts.Add(_context.Posts.Include(p => p.Blog).Include(p => p.Comments).FirstOrDefault(p => p.Id == postId));
            }

            int totalReplies = 0;
            foreach (var comment in post.Comments)
            {
                totalReplies += comment.Replies.Count();
            }

            var dataVM = new PostDetailViewModel()
            {
                Post = post,
                Tags = post.Tags
                        .Select(t => t.Text.ToLower())
                        .ToList(),
                RelatedPosts = relatedPosts
            };

            ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(dataVM);
        } 

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                var authorId = _userManager.GetUserId(User);
                post.AuthorId = authorId;

                //Use the _imageService to store the incoming user specified image
                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                //Create the slug and determine if it is unique
                var slug = _slugService.UrlFriendly(post.Title);

                //Create a variable to store whether an error has occured
                var validationError = false;

				if (string.IsNullOrEmpty(slug))
				{
					validationError = true;
					ModelState.AddModelError("", "The Title you provided cannot be used as it results in an empty slug."); 
				}

				//Detect incoming duplicate Slugs
				else if (!_slugService.IsUnique(slug))
				{
                    validationError = true;
					ModelState.AddModelError("Title", "The Title you provided cannot be used as it results in a duplicate slug.");
				}

                else if (slug.Contains("test"))
                {
                    validationError = true;
					ModelState.AddModelError("", "Uh-oh are you testing again??");
					ModelState.AddModelError("Title", "The Title cannot contain the word 'test'");
				}

                if (validationError)
                {
					ViewData["TagValues"] = string.Join(",", tagValues);
					return View(post);
				}

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                //Loop over the incoming list of strings
                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        AuthorId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);

            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Tags)
                .Include(p => p.RelatedPosts)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

			var postsIds = new List<int>();

			//Get post related articles and add each into postsIds list
			post.RelatedPosts.ToList().ForEach(result => postsIds.Add(result.ArticleId));

			ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Title", post.Id);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            var relatedPosts = _context.Posts.Select(p => new SelectListItem { Text = p.Title, Value = p.Id.ToString() }).ToList();

            //disable related post option in select list that has same name as current post
            foreach (var optionItem in relatedPosts)
            {
                if (optionItem.Text == post.Title)
                {
					optionItem.Disabled = true;
                }
            }

			var postEditVM = new PostEditViewModel()
            {
				Post = post,
                drpPosts = relatedPosts,
                PostsIds = postsIds
			};

			return View(postEditVM);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostEditViewModel model, IFormFile? newImage, List<string> tagValues)
        {
            if (id != model.Post.Id)
            {
                return NotFound();
            }

            if (id > 0)
            {
                try
                {
                    var newPost = await _context.Posts
                        .Include(p => p.Tags)
                        .Include(p => p.RelatedPosts)
                        .FirstOrDefaultAsync(p => p.Id == model.Post.Id);

                    newPost.Updated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    newPost.Title = model.Post.Title;
                    newPost.Abstract = model.Post.Abstract;
                    newPost.Content = model.Post.Content;
                    newPost.ReadyStatus = model.Post.ReadyStatus;

                    var newSlug = _slugService.UrlFriendly(model.Post.Title);
                    if (newSlug != newPost.Slug)
                    {
                        if (_slugService.IsUnique(newSlug))
                        {
                            newPost.Title = model.Post.Title;
                            newPost.Slug = newSlug;
                        }
                        else
                        {
                            ModelState.AddModelError("Title", "This Title cannot be used as it results in a duplicate slug");
							ViewData["TagValues"] = string.Join(",", model.Post.Tags.Select(t => t.Text));
							return View(model.Post);
                        }
                    }

                    if (newImage is not null)
                    {
                        newPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        newPost.ContentType = _imageService.ContentType(newImage);
                    }

                    //Remove all Tags previously associated with this Post
                    _context.Tags.RemoveRange(newPost.Tags);

                    //Add in the new Tags from the Edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = model.Post.Id,
                            AuthorId = newPost.AuthorId,
                            Text = tagText
                        });
                    }

                    //Remove all related Posts previously associated with this Post
                    List<Related> relatedPosts = new List<Related>();
                    newPost.RelatedPosts.ToList().ForEach(result => relatedPosts.Add(result));
                    _context.Relateds.RemoveRange(relatedPosts);
                    await _context.SaveChangesAsync();

                    //Add in the new related Posts from the Edit form
                    if(model.PostsIds.Count() > 0)
                    {
						relatedPosts = new List<Related>();

                        foreach (var postId in model.PostsIds)
                        {
                            relatedPosts.Add(new Related { PostId = id, ArticleId = postId });
                        }
                        newPost.RelatedPosts = relatedPosts;
					}
					await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(model.Post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View("Index", _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.Author)
                .Include(p => p.Tags)
                .Include(p => p.RelatedPosts)
                .Include(p => p.Comments));
            } 
            return View(model.Post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
