using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEBlog.Models
{
    public class Related
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int ArticleId { get; set; }

        //Navigation properties
        public virtual Post? Post { get; set; }
	}
}
