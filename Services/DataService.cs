using Microsoft.AspNetCore.Identity;
using CEBlog.Data;
using CEBlog.Enums;
using CEBlog.Models;

namespace CEBlog.Services
{
	public class DataService
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<BlogUser> _userManager;

		public DataService(ApplicationDbContext dbContext,
						   RoleManager<IdentityRole> roleManager,
						   UserManager<BlogUser> userManager)
		{
			_dbContext = dbContext;
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task ManageDataAsync()
		{
			//Task 1: Seed a few Roles into the system
			await SeedRolesAsync();

			//Task 2: Seed a few users into the system
			await SeedUsersAsync();
		}

		private async Task SeedRolesAsync()
		{
			//If there are already Roles in the system, do nothing. 
			if (_dbContext.Roles.Any())
			{
				return;
			}

			//Otherwise we want to create a few Roles
			foreach (var role in Enum.GetNames(typeof(BlogRole)))
			{
				//I need to use the Role Manager to create roles
				await _roleManager.CreateAsync(new IdentityRole(role));
			}
		}

		private async Task SeedUsersAsync()
		{
			//If there are already Roles in the system, do nothing. 
			if (_dbContext.Users.Any())
			{
				return;
			}

			//Step 1: Create a new instance of BlogUser
			var adminUser = new BlogUser()
			{
				Email = "chikeredev@gmail.com",
				UserName = "chikeredev@gmail.com",
				FirstName = "Chikere",
				LastName = "Eze",
				DisplayName = "Manager",
				PhoneNumber = "(800) 555-1212",
				EmailConfirmed = true
			};

			//Step 2: Use the UserManager to create a new user that is defined by adminUser
			await _userManager.CreateAsync(adminUser, "Learntocode1!");

			//Step 3: Add this new user to the Administrator role
			await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

			//Step 1 repeat: Create the moderator user
			var modUser = new BlogUser()
			{
				Email = "chikere.ezekannagha@gmail.com",
				UserName = "chikere.ezekannagha@gmail.com",
				FirstName = "Chik",
				LastName = "Eze",
				DisplayName = "Asst Manager",
				PhoneNumber = "(800) 555-1213",
				EmailConfirmed = true
			};

			await _userManager.CreateAsync(modUser, "Learntocode1!");
			await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
		
		} 
	}
}
