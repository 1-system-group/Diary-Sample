// -----------------------------------------------------------------------
// <copyright file="ManageController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Diary_Sample.Common.ResultType;
namespace Diary_Sample.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ManageController> _logger;
        public ManageController(
            ILogger<ManageController> logger,
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ManageViewModel(_userManager, 1));
        }
        [HttpGet]
        public IActionResult NewEntry()
        {
            // アカウント新規登録画面へ遷移
            return View("CreateAccount", new CreateAccountViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!ModelState.IsValid)
            {
                model.Notification = string.Empty;
                model.NotificationType = None;
                return View("CreateAccount", model);
            }

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                // FIXME メール認証を作成したら消す
                EmailConfirmed = true,
            };

            if (model.Password1 != model.Password2)
            {
                // パスワード不一致エラー
                model.Notification = "パスワードとパスワード（再入力）が不一致です。";
                model.NotificationType = Error;
                return View("CreateAccount", model);
            }

            IdentityResult result = await _userManager.PasswordValidators[0].ValidateAsync(_userManager, user, model.Password1).ConfigureAwait(false);
            if (result != IdentityResult.Success)
            {
                // パスワードエラー
                model.Notification = "パスワードが不正です。";
                model.NotificationType = Error;
                return View("CreateAccount", model);
            }

            // アカウント登録
            result = await _userManager.CreateAsync(user, model.Password1).ConfigureAwait(false);
            if (result == IdentityResult.Success)
            {
                // 登録成功の場合はアカウント一覧画面へ
                return View("Index", new ManageViewModel(_userManager, 1)
                {
                    Notification = "登録が完了しました。",
                    NotificationType = Normal,
                });
            }
            else
            {
                // 登録失敗
                model.Notification = "入力されたアカウントはすでに登録されています。";
                model.NotificationType = Error;
                return View("CreateAccount", model);
            }
        }
        [HttpGet]
        public IActionResult Paging(int page)
        {
            return View("Index", new ManageViewModel(_userManager, page));
        }
        [HttpPost]
        public async Task<IActionResult> UnlockAsync(string unlockId)
        {
            IdentityUser user = await _userManager.FindByIdAsync(unlockId).ConfigureAwait(false);
            // ロック解除
            await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
            // ロックを解除したユーザのページを表示
            return View("Index", new ManageViewModel(_userManager, user));
        }
    }
}