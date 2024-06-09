// -----------------------------------------------------------------------
// <copyright file="CreateController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Controllers;

[Authorize]
public class CreateController : Controller
{
    private readonly ILogger<CreateController> _logger;
    private readonly ICreateService _service;
    public CreateController(ILogger<CreateController> logger, ICreateService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_service.Index());
    }

    [HttpPost]
    public IActionResult Create(CreateViewModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }
        if (_service.Create(model))
        {
            // 登録成功の場合はメニュー画面へ
            TempData["notification"] = "登録が完了しました。";
            return RedirectToAction("index", "Menu");
        }
        else
        {
            // 登録失敗
            model.Notification = "登録に失敗しました";
            return View("Index", model);
        }
    }

    [HttpGet]
    public IActionResult Back()
    {
        // メニュー画面へ遷移
        return RedirectToAction("index", "Menu");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}