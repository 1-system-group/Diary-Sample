// -----------------------------------------------------------------------
// <copyright file="ReferController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Common;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers;

[Authorize]
public class ReferController : Controller
{
    private readonly IReferService _service;

    private const string IdNoFoundMessage = "日記が見つかりませんでした。";

    public ReferController(IReferService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Index(string id)
    {
        int numId = CommonUtil.ConvAcceptId(id);
        if (numId == -1)
        {
            return IdNotFound();
        }

        var viewModel = _service.GetDiary(numId);
        if (viewModel == null)
        {
            return IdNotFound();
        }

        return View(viewModel);
    }

    private IActionResult IdNotFound()
    {
        TempData["notification"] = IdNoFoundMessage;
        return RedirectToAction("Index", "Menu");
    }
}