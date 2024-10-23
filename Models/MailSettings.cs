namespace CEBlog.Models
{
    public class MailSettings
    {
        //So we can configure and use smtp serve
        //from google for example
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string MailPassword { get; set; }
        public string MailHost { get; set; }
        public int MailPort { get; set; }
    }
}
