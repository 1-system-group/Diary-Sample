// -----------------------------------------------------------------------
// <copyright file="ForgotPassword.cshtml.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Infra.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Diary_Sample.Pages.Auth
{
    [AllowAnonymous]
    public class ForgotPassword : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPassword(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "Eメールは必須です。")]
            [EmailAddress(ErrorMessage = "Eメールアドレスの形式で入力してください。")]
            [Display(Name = "Eメール")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email).ConfigureAwait(false);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
                {
                    // ユーザーが存在しない、または確認されていないことを明かさない
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Auth/ResetPassword",
                    pageHandler: null,
                    values: new { code, Input.Email },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    user.UserName,
                    "Reset Password",
                    $"パスワードをリセットするにはリンクをクリックしてください。{HtmlEncoder.Default.Encode(callbackUrl)}",
                    $"パスワードをリセットするにはリンクをクリックしてください。{callbackUrl}").ConfigureAwait(false);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}