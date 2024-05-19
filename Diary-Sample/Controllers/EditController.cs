// -----------------------------------------------------------------------
// <copyright file="EditController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Common;
using Diary_Sample.Models;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers;

[Authorize]
public class EditController : Controller
{
    private readonly IEditService _service;

    private const string ErrorPath = "Views/Shared/Error.cshtml";

    private const string DeleteOkMessage = "削除しました。";

    private const string IdNoFoundMessage = "日記が見つかりませんでした。";

    private const string EditFailMessage = "更新できませんでした。";

    private const string DeleteFailMessage = "すでに削除されています。";

    public EditController(IEditService service)
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

    [HttpPost]
    public IActionResult Edit(EditViewModel editViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", editViewModel);
        }

        var ret = _service.UpdateDiary(editViewModel);
        if (!ret)
        {
            TempData["notification"] = EditFailMessage;
            return RedirectToAction("Index", "Menu");
        }

        return RedirectToAction("Index", "Refer", new { id = editViewModel.Id });

    }

    [HttpPost]
    public IActionResult Delete(EditViewModel editViewModel)
    {
        int numId = CommonUtil.ConvAcceptId(editViewModel.Id);
        if (numId == -1)
        {
            return View(ErrorPath, new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        var ret = _service.DeleteDiary(numId);
        if (!ret)
        {
            TempData["notification"] = DeleteFailMessage;
            return RedirectToAction("Index", "Menu");
        }

        TempData["notification"] = DeleteOkMessage;
        return RedirectToAction("Index", "Menu");
    }

    private IActionResult IdNotFound()
    {
        TempData["notification"] = IdNoFoundMessage;
        return RedirectToAction("Index", "Menu");
    }
}