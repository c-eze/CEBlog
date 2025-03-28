﻿using CEBlog.Enums;
using System.ComponentModel.DataAnnotations;

namespace CEBlog.Models
{
	public class Reply
	{
		public int Id { get; set; }
		public int PostId { get; set; }
		public string? AuthorId { get; set; }
		public string? ModeratorId { get; set; }
		public int CommentId { get; set; }

		[Required]
		[StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
		[Display(Name = "Comment")]
		public string? Body { get; set; }

		public DateTime Created { get; set; }
		public DateTime? Updated { get; set; }
		public DateTime? Moderated { get; set; }
		public DateTime? Deleted { get; set; }

		[StringLength(500, ErrorMessage = "The {0} must be at least {2} and no more than {1} characters long", MinimumLength = 2)]
		[Display(Name = "Moderated Comment")]
		public string? ModeratedBody { get; set; }

		public ModerationType ModerationType { get; set; }

		public CommentStatus CommentStatus { get; set; }

		//Navigation properties
		public virtual Post? Post { get; set; }
		public virtual BlogUser? Author { get; set; }
		public virtual BlogUser? Moderator { get; set; }
		public virtual Comment? Comment { get; set; }
	}
}
