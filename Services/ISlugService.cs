﻿namespace CEBlog.Services
{
	public interface ISlugService
	{
		string UrlFriendly(string title);
		bool IsUnique(string slug);
	}
}
