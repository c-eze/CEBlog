using System.ComponentModel.DataAnnotations;

namespace CEBlog.ViewModels
{
    public class SubscribeTo
    {
        [Required]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string Email { get; set; }
    }
}
