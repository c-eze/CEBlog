using CEBlog.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CEBlog.Models.DTOs
{
	public class PostDTO
	{
		public string Title { get; set; }

		public DateTime? Updated { get; set; }

		public string Slug { get; set; }

		public byte[] ImageData { get; set; } 

		public IEnumerable<TagDTO> Tags { get; set; } 
	}
}
