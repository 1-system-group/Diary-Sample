// -----------------------------------------------------------------------
// <copyright file="UserAdminAccountDeleteController.cs" company="1-system-group">
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
public class UserAdminAccountDeleteController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<UserAdminAccountDeleteController> _logger;

    private const string DeleteOkMessage = "削除しました。";
    private const string DeleteNgMessage = "エラーが発生して削除できませんでした。";

    public UserAdminAccountDeleteController(ILogger<UserAdminAccountDeleteController> logger,
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

        UserAdminAccountDeleteViewModel userAdminAccountDeleteViewModel = new UserAdminAccountDeleteViewModel
        {
            UserId = user.Id,
            Password = string.Empty,
        };

        return View("Index", userAdminAccountDeleteViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Delete(UserAdminAccountDeleteViewModel userAdminAccountDeleteViewModel)
    {

        string userId = userAdminAccountDeleteViewModel.UserId;
        string password = userAdminAccountDeleteViewModel.Password;

        if (!ModelState.IsValid)
        {
            return UpadateError(userAdminAccountDeleteViewModel, DeleteNgMessage);
        }

        if (userId == null)
        {
            return UpadateError(userAdminAccountDeleteViewModel, DeleteNgMessage);
        }

        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null)
        {
            return UpadateError(userAdminAccountDeleteViewModel, DeleteNgMessage);
        }

        bool retPassCheck = await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false);
        if (!retPassCheck)
        {
            return UpadateError(userAdminAccountDeleteViewModel, DeleteNgMessage);
        }

        var retDelete = await _userManager.DeleteAsync(user).ConfigureAwait(false);
        if (!retDelete.Succeeded)
        {
            return UpadateError(userAdminAccountDeleteViewModel, DeleteNgMessage);
        }

        await _signInManager.SignOutAsync().ConfigureAwait(false);

        return RedirectToAction("Index", "Auth");
    }

    private IActionResult UpadateError(UserAdminAccountDeleteViewModel userAdminAccountDeleteViewModel, string message)
    {
        userAdminAccountDeleteViewModel.Notification = message;

        return View("Index", userAdminAccountDeleteViewModel);
    }

}