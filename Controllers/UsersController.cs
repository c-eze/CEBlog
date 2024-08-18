using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using CEBlog.Data;
using CEBlog.Enums;
using CEBlog.Models;
using CEBlog.ViewModels;
using X.PagedList.Extensions;

namespace CEBlog.Controllers
{
	public class UsersController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<BlogUser> _userManager;

        public UsersController(ApplicationDbContext context,
							   UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
		{
			var users = _context.Users.Include(p => p.Posts);
			var query = from user in users
						join userRole in _context.UserRoles
							on user.Id equals userRole.UserId into grouping
						from userRole in grouping.DefaultIfEmpty()
						join role in _context.Roles
							on userRole.RoleId equals role.Id into grouping2
						from role in grouping2.DefaultIfEmpty()
						select new UserIndexViewModel
						{
							DisplayName = user.DisplayName,
							UserName = user.UserName,
							Email = user.Email,
							Role = role.Name,
							TotalPosts = user.Posts.Count(),
							AuthorId = user.Posts.FirstOrDefault().AuthorId
						};

			return View(await query.ToListAsync());
		}

		
		public async Task<IActionResult> Edit(string? userName)
		{
            if (string.IsNullOrEmpty(userName))
            {
				return NotFound(); 
            }

			var enumTypes = typeof(BlogRole);

            var user = from users in _context.Users
					   where users.UserName == userName
					   join userRoles in _context.UserRoles
							on users.Id equals userRoles.UserId into grouping
					   from userRoles in grouping.DefaultIfEmpty()
					   join roles in _context.Roles
							on userRoles.RoleId equals roles.Id into grouping2
					   from roles in grouping2.DefaultIfEmpty()
					   select new UserEditViewModel
					   {
						   UserName = users.UserName,
						   DisplayName = users.DisplayName,
						   FirstName = users.FirstName,
						   LastName = users.LastName,
						   Email = users.Email,
						   PhoneNumber = users.PhoneNumber,
						   Role = (BlogRole) Enum.Parse(enumTypes, roles.Name ?? "Subscriber", true) 
					   };

            return View(await user.FirstOrDefaultAsync());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string userName, [Bind("UserName,Role,DisplayName,FirstName,LastName,Email,PhoneNumber")] UserEditViewModel user)
		{
            if (userName != user.UserName)
            {
				return NotFound();
            }

            if (ModelState.IsValid)
            {
				var newUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                try
                {
                    newUser.DisplayName = user.DisplayName;
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Email = user.Email;
                    newUser.PhoneNumber = user.PhoneNumber;

                    await _userManager.UpdateAsync(newUser);

					var roles = await _userManager.GetRolesAsync(newUser);
                    await _userManager.RemoveFromRolesAsync(newUser, roles);

					await _userManager.AddToRoleAsync(newUser, user.Role.ToString());
                }
                catch (Exception)
				{
					throw;
				}
				return RedirectToAction("Index", "Users");
            }


            return View(user);
		}

		public async Task<IActionResult> Details(string? authorId)
        {
            var pageNumber = 1;
            var pageSize = 5;

            //var posts = _context.Posts.Where(p => p.BlogId == id).ToList();
            var posts = _context.Posts
                .Where(p => p.AuthorId == authorId && p.ReadyStatus == ReadyStatus.ProductionReady)
                .OrderByDescending(p => p.Created)
                .ToPagedList(pageNumber, pageSize);
            return View(posts);
		}
	}
}
