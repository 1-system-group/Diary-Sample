// -----------------------------------------------------------------------
// <copyright file="ResetPasswordApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Text;
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
public class ResetPasswordApiController : ControllerBase, IResetPasswordApiController
{
    private readonly ILogger<ResetPasswordApiController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    public ResetPasswordApiController(
        ILogger<ResetPasswordApiController> logger,
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <inheritdoc />
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "入力内容が不正です。" });
        }

        var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
        if (user == null)
        {
            // ユーザーが存在しないことを明かさない
            return Ok(new { message = "パスワードをリセットしました。" });
        }

        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ResetPasswordAsync(user, code, model.Password).ConfigureAwait(false);
        if (result.Succeeded)
        {
            return Ok(new { message = "パスワードをリセットしました。" });
        }

        foreach (var error in result.Errors)
        {
            _logger.LogWarning("パスワードリセットエラー: {Description}", error.Description);
        }

        return BadRequest(new { message = "パスワードのリセットに失敗しました。" });
    }
}