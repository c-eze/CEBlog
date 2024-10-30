using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CEBlog.Data;
using CEBlog.Models;
using CEBlog.Enums;

namespace CEBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

		[HttpGet]
		[Route("/portfolio/{count}")]
		public async Task<ActionResult<IEnumerable<Post>>> GetPortfolioPosts(int? count)
		{
			if (_context.Posts == null || count == null)
			{
				return NotFound();
			}

            IEnumerable<Post>? result = await _context.Posts
                .Include(p => p.Tags)
                .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady)
				.OrderByDescending(p => p.Created)
				.Take(count.Value)
				.ToListAsync();

            //IEnumerable<Post>? result = await _context.Posts
            //						.Where(p => p.Id == 5 || p.Id == 8 || p.Id == 7)
            //						.OrderByDescending(p => p.Title)
            //						.ToListAsync();
            // change above line to include specific posts

            if (result.Any())
			{
				return Ok(result);
			}

			return BadRequest();
		}

		// GET: api/Posts
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
          if (_context.Posts == null)
          {
              return NotFound();
          }
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
          if (_context.Posts == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
          }
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.Id }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
