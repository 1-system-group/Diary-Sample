// -----------------------------------------------------------------------
// <copyright file="ResetPassword.cshtml.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Diary_Sample.Pages.Auth
{
    [AllowAnonymous]
    public class ResetPassword : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPassword(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Eメールは必須です。")]
            [EmailAddress(ErrorMessage = "Eメールアドレスの形式で入力してください。")]
            [Display(Name = "Eメール")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "パスワードは必須です。")]
            [StringLength(100, ErrorMessage = "{0}は{2}〜{1}文字で入力してください。", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "パスワード")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "確認用パスワードは必須です。")]
            [DataType(DataType.Password)]
            [Display(Name = "確認用パスワード")]
            [Compare("Password", ErrorMessage = "パスワードと確認用パスワードが不一致です。")]
            public string ConfirmPassword { get; set; } = string.Empty;

            public string Code { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code)),
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email).ConfigureAwait(false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}