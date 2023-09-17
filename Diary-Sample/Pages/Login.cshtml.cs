// -----------------------------------------------------------------------
// <copyright file="Login.cshtml.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Diary_Sample
{
    [AllowAnonymous]
    public class Login : PageModel
    {
        private const string LoginMessage = "User logged in.";
        private const string LockedoutMessage = "User account locked out.";
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<Login> _logger;

        public Login(
            SignInManager<IdentityUser> signInManager,
            ILogger<Login> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public string ReturnUrl { get; set; } = string.Empty;
        // public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Eメールは必須です。")]
            [EmailAddress(ErrorMessage = "Eメールアドレスの形式で入力してください。")]
            [Display(Name = "Eメール")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "パスワードは必須です。")]
            [DataType(DataType.Password)]
            [Display(Name = "パスワード")]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "次回から自動ログインする")]
            public bool RememberMe { get; set; }
            public string Notification { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);
            // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(LoginMessage);
                    return RedirectToAction("Index", "Menu");
                }
                // if (result.RequiresTwoFactor)
                // {
                //     return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                // }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(LockedoutMessage);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    Input.Notification = "ログインに失敗しました";
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}