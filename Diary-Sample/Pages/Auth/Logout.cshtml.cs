// -----------------------------------------------------------------------
// <copyright file="Logout.cshtml.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Pages.Auth
{
    [AllowAnonymous]
    public class Logout : PageModel
    {
        private const string LogoutLogMessage = "User logged out.";
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<Logout> _logger;

        public Logout(SignInManager<IdentityUser> signInManager, ILogger<Logout> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            _logger.LogInformation(LogoutLogMessage);
            return LocalRedirect("/");
        }
    }
}