// -----------------------------------------------------------------------
// <copyright file="ApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Infra;
using Diary_Sample.Models;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Controllers;

[ApiController]
[Route("api/v1/[action]")]
[Produces("application/json")]
public class ApiController : ControllerBase, IApiController
{
    private readonly ILogger<ApiController> _logger;
    private readonly IApiService _service;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtHandler _jwtHandler;

    public ApiController(ILogger<ApiController> logger, IApiService service,
        UserManager<IdentityUser> userManager,
        IJwtHandler jwtHandler)
    {
        _logger = logger;
        _service = service;
        _userManager = userManager;
        _jwtHandler = jwtHandler;
    }

    /// <inheritdoc />
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public ActionResult<string> Login([FromBody] AuthModel model)
    {
        IdentityUser user = Task.Run(() => { return _userManager.FindByNameAsync(model.email).ConfigureAwait(false); }).Result.GetAwaiter().GetResult();
        if (user == null)
        {
            return Unauthorized();
        }

        if (user.LockoutEnd != null)
        {
            // ロックされていたら認証エラー
            return Unauthorized();
        }

        bool isPasswordOk = Task.Run(() =>
        {
            return _userManager.CheckPasswordAsync(user, model.password).ConfigureAwait(false);
        }).Result.GetAwaiter().GetResult();
        if (isPasswordOk)
        {
            // 認証トークンを発行する
            var roles = Task.Run(() =>
            {
                return _userManager.GetRolesAsync(user).ConfigureAwait(false);
            }).Result.GetAwaiter().GetResult();
            var token = _jwtHandler.GenerateEncodedToken(user.UserName, model.deviceId, roles);
            return Ok(token);
        }

        // 認証失敗回数をインクリメントし、最大試行回数行った場合はロックする
        _userManager.AccessFailedAsync(user).ConfigureAwait(false);
        return Unauthorized();
    }

    /// <inheritdoc />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{page}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public ActionResult<List<DiaryRow>> Lists(int page) =>
        page > 0 ? (ActionResult)Ok(_service.Lists(page)) : BadRequest();

    /// <inheritdoc />
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<int> Counts() => Ok(_service.Counts());

    /// <inheritdoc />
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DiaryModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public ActionResult<DiaryModel> Diary(int id)
    {
        DiaryModel diary = _service.Diary(id);
        if (diary == null)
        {
            return NotFound();
        }

        return Ok(diary);
    }
}