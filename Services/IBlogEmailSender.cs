﻿using Microsoft.AspNetCore.Identity.UI.Services;

namespace CEBlog.Services
{
	public interface IBlogEmailSender : IEmailSender
	{
		Task SendContactEmailAsync(string emailFrom, string htmlMessage);
        Task SendSubscribeEmailAsync(string emailFrom);
    }
}
