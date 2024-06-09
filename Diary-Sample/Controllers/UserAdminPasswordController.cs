// -----------------------------------------------------------------------
// <copyright file="UserAdminPasswordController.cs" company="1-system-group">
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
public class UserAdminPasswordController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<UserAdminPasswordController> _logger;

    private const string EditOkMessage = "更新しました。";
    private const string EditNgMessage = "エラーが発生して更新できませんでした。";
    private const string PasswordMismatch = "変更後のパスワードと変更後のパスワード（再入力）が不一致です。";
    private const string PasswordMisstake = "現在のパスワードが間違っています。";
    private const string PasswordIllegal = "変更後のパスワードが不正です。";

    public UserAdminPasswordController(ILogger<UserAdminPasswordController> logger,
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

        UserAdminPasswordViewModel userAdminPasswordViewModel = new UserAdminPasswordViewModel
        {
            OldPassword = string.Empty,
            NewPassword1 = string.Empty,
            NewPassword2 = string.Empty,
        };

        return View("Index", userAdminPasswordViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(UserAdminPasswordViewModel userAdminPasswordViewModel)
    {
        string oldPassword = userAdminPasswordViewModel.OldPassword;
        string newPassword1 = userAdminPasswordViewModel.NewPassword1;
        string newPassword2 = userAdminPasswordViewModel.NewPassword2;

        if (!ModelState.IsValid)
        {
            return UpadateError(userAdminPasswordViewModel, EditNgMessage);
        }

        if (!newPassword1.Equals(newPassword2))
        {
            return UpadateError(userAdminPasswordViewModel, PasswordMismatch);
        }

        var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
        if (user == null)
        {
            return UpadateError(userAdminPasswordViewModel, EditNgMessage);
        }

        // 念のため変更前のパスワードをチェックする
        bool retCheckPass = await _userManager.CheckPasswordAsync(user, oldPassword).ConfigureAwait(false);
        if (!retCheckPass)
        {
            return UpadateError(userAdminPasswordViewModel, PasswordMisstake);
        }

        var retValidPass = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, newPassword1).ConfigureAwait(false);
        if (!retValidPass.Succeeded)
        {
            return UpadateError(userAdminPasswordViewModel, PasswordIllegal);
        }

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword1).ConfigureAwait(false);

        userAdminPasswordViewModel.Notification = EditOkMessage;
        userAdminPasswordViewModel.UpdateResult = true;

        return View("Index", userAdminPasswordViewModel);

    }

    private IActionResult UpadateError(UserAdminPasswordViewModel userAdminPasswordViewModel, string message)
    {
        userAdminPasswordViewModel.Notification = message;
        userAdminPasswordViewModel.UpdateResult = false;

        return View("Index", userAdminPasswordViewModel);

    }

}