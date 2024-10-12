using CEBlog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CEBlog.ViewModels
{
    public class PostEditViewModel
    {
        public Post Post { get; set; }

		[Display(Name = "Related Articles")]
		public List<SelectListItem> drpPosts { get; set; } 
        public IEnumerable<int> PostsIds { get; set; } 
    }
}
