namespace CEBlog.Services
{
    public interface INavigationService
    {
        string Previous(int id);
        string Next(int id);
    }
}
