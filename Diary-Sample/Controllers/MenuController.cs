// -----------------------------------------------------------------------
// <copyright file="MenuController.cs" company="1-system-group">
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
public class MenuController : Controller
{
    private readonly ILogger<MenuController> _logger;
    private readonly IMenuService _service;
    public MenuController(ILogger<MenuController> logger, IMenuService service)
    {
        _logger = logger;
        _service = service;
    }
    [HttpGet]
    public IActionResult Index()
    {
        string notification = TempData?["notification"] as string ?? string.Empty;
        return View(_service.Index(notification));
    }
    [HttpGet]
    public IActionResult Paging(int page)
    {
        return View("Index", _service.Paging(page));
    }

    [HttpGet]
    public IActionResult NewEntry()
    {
        // 新規登録画面へ遷移
        return RedirectToAction("index", "Create");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}