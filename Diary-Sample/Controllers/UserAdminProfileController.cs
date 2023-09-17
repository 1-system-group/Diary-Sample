// -----------------------------------------------------------------------
// <copyright file="UserAdminProfileController.cs" company="1-system-group">
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
public class UserAdminProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<UserAdminProfileController> _logger;

    private const string EditOkMessage = "更新しました。";
    private const string EditNgMessage = "エラーが発生して更新できませんでした。";

    public UserAdminProfileController(ILogger<UserAdminProfileController> logger,
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

        UserAdminProfileViewModel userAdminProfileViewModel = new UserAdminProfileViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            PhoneNumber = user.PhoneNumber,
        };

        return View("Index", userAdminProfileViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserAdminProfileViewModel userAdminProfileViewModel)
    {
        string userName = userAdminProfileViewModel.UserName;
        string phoneNumber = userAdminProfileViewModel.PhoneNumber;

        if (!ModelState.IsValid)
        {
            return UpadateError(userAdminProfileViewModel, EditNgMessage);
        }

        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null)
        {
            return UpadateError(userAdminProfileViewModel, EditNgMessage);
        }

        var identityResult = await _userManager.SetUserNameAsync(user, userName).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            return UpadateError(userAdminProfileViewModel, EditNgMessage);
        }
        identityResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber).ConfigureAwait(false);
        if (!identityResult.Succeeded)
        {
            return UpadateError(userAdminProfileViewModel, EditNgMessage);
        }

        userAdminProfileViewModel.Notification = EditOkMessage;
        userAdminProfileViewModel.UpdateResult = true;

        return View("Index", userAdminProfileViewModel);
    }

    private IActionResult UpadateError(UserAdminProfileViewModel userAdminProfileViewModel, string message)
    {
        userAdminProfileViewModel.Notification = message;
        userAdminProfileViewModel.UpdateResult = false;

        return View("Index", userAdminProfileViewModel);
    }
}