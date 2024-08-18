using System.ComponentModel.DataAnnotations;
using CEBlog.Models;

namespace CEBlog.ViewModels
{
	public class UserIndexViewModel
	{
		[Display(Name = "Display Name")]
		public string DisplayName { get; set; }

		[Display(Name = "User Name")]
		public string UserName { get; set; }

        public string Email { get; set; }
		public string Role { get; set; }

		[Display(Name = "Posts")]
		public int? TotalPosts { get; set; }
        public string AuthorId { get; set; }
    }
}
