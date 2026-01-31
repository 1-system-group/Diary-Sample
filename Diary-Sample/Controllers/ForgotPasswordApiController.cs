// -----------------------------------------------------------------------
// <copyright file="ForgotPasswordApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Text;
using Diary_Sample.Infra.Mail;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Controllers;

[ApiController]
[Route("api/v1/[action]")]
[Produces("application/json")]
public class ForgotPasswordApiController : ControllerBase, IForgotPasswordApiController
{
    private readonly ILogger<ForgotPasswordApiController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly string _clientBaseUrl;

    public ForgotPasswordApiController(
        ILogger<ForgotPasswordApiController> logger,
        UserManager<IdentityUser> userManager,
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _emailSender = emailSender;
        _configuration = configuration;

        _clientBaseUrl = Environment.GetEnvironmentVariable("CLIENT_BASE_URL") ?? _configuration["ClientBaseUrl"];
        if (string.IsNullOrEmpty(_clientBaseUrl))
        {
            _logger.LogError("_clientBaseUrl is not configured");
            throw new System.Exception();
        }
    }

    /// <inheritdoc />
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "入力内容が不正です。" });
        }

        var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false)))
        {
            // ユーザーが存在しない、または確認されていないことを明かさない
            return Ok(new { message = "パスワードリセットメールを送信しました。" });
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var resetUrl = $"{_clientBaseUrl}/reset-password?code={code}&email={model.Email}";

        await _emailSender.SendEmailAsync(
            model.Email,
            user.UserName ?? string.Empty,
            "Reset Password",
            $"パスワードをリセットするにはリンクをクリックしてください。{resetUrl}",
            $"パスワードをリセットするにはリンクをクリックしてください。<a href=\"{resetUrl}\">こちら</a>").ConfigureAwait(false);

        return Ok(new { message = "パスワードリセットメールを送信しました。" });
    }
}