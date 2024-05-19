// -----------------------------------------------------------------------
// <copyright file="UserAdminAccountController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Controllers;

[Authorize]
public class UserAdminAccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<UserAdminAccountController> _logger;

    private const string EditOkMessage = "更新しました。";
    private const string DeleteOkMessage = "削除しました。";
    private const string EditNgMessage = "エラーが発生して更新できませんでした。";
    private const string DeleteNgMessage = "エラーが発生して削除できませんでした。";

    public UserAdminAccountController(ILogger<UserAdminAccountController> logger,
                                SignInManager<IdentityUser> signInManager,
                                UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

        UserAdminAccountViewModel userAdminAccountViewModel = new UserAdminAccountViewModel();

        return View("Index", userAdminAccountViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Download()
    {
        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

        var value = "{\"Id\":" + user.Id +
                    ",\"UserName\":" + user.UserName +
                    ",\"Email\":" + user.Email +
                    ",\"EmailConfirmed\":" + user.EmailConfirmed +
                    ",\"PhoneNumber\":" + user.PhoneNumber +
                    ",\"PhoneNumberConfirmed\":" + user.PhoneNumberConfirmed + "}";
        var filename = "PersonalData.json";

        return File(new MemoryStream(Encoding.UTF8.GetBytes(value)), "application/json", filename);
    }
}