using CEBlog.Models;

namespace CEBlog.ViewModels
{
    public class PostDetailViewModel
    {
        public Post Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        public List<Post> RelatedPosts { get; set; } = new List<Post>();
        public int TotalComments { get; set; }
    }
}
