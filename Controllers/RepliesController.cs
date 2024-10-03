using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CEBlog.Data;
using CEBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using CEBlog.Services;

namespace CEBlog.Controllers
{
    public class RepliesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogEmailSender _emailSender;

        public RepliesController(ApplicationDbContext context,
                                 UserManager<BlogUser> userManager,
                                 IBlogEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Replies
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reply.Include(r => r.Author).Include(r => r.Moderator).Include(r => r.Post);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Replies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }

            var reply = await _context.Reply
                .Include(r => r.Author)
                .Include(r => r.Moderator)
                .Include(r => r.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reply == null)
            {
                return NotFound();
            }

            return View(reply);
        }

        // GET: Replies/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract");
            return View();
        }

        // POST: Replies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PostId,CommentId,Body")] Reply reply, string slug)
        {
            if (ModelState.IsValid)
            {
                reply.AuthorId = _userManager.GetUserId(User);
                reply.Created = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                reply.CommentStatus = Enums.CommentStatus.New;

                _context.Add(reply);
                await _context.SaveChangesAsync();


                string subject = $"[Chikere.Dev] New comment";
                string message = $"<p>A new comment on the post '{slug}' is waiting for your approval</p><p>https://chikere.dev/{slug}</p><br/><p>Email: {_userManager.GetUserName(User)}</p><p>Comment: {reply.Body}</p> ";
                await _emailSender.SendEmailAsync("chikeredev@gmail.com", subject, message);

                return RedirectToAction("Details", "Posts", new { slug }, "commentSection");
            }
 
            return View(reply);
        }

        // GET: Replies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }

            var reply = await _context.Reply.FindAsync(id);
            if (reply == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", reply.AuthorId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", reply.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", reply.PostId);
            return View(reply);
        }

        // POST: Replies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PostId,AuthorId,ModeratorId,CommentId,Body,Created,Updated,Moderated,Deleted,ModeratedBody,ModerationType,CommentStatus")] Reply reply)
        {
            if (id != reply.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reply);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReplyExists(reply.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", reply.AuthorId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", reply.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", reply.PostId);
            return View(reply);
        }

        // GET: Replies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }

            var reply = await _context.Reply
                .Include(r => r.Author)
                .Include(r => r.Moderator)
                .Include(r => r.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reply == null)
            {
                return NotFound();
            }

            return View(reply);
        }

        // POST: Replies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reply == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reply'  is null.");
            }
            var reply = await _context.Reply.FindAsync(id);
            if (reply != null)
            {
                _context.Reply.Remove(reply);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReplyExists(int id)
        {
          return (_context.Reply?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
