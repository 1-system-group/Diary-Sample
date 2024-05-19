// -----------------------------------------------------------------------
// <copyright file="EditControllerTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Controllers;
using Diary_Sample.Models;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Diary_Sample_Test.Controllers;

public class EditControllerTest
{
    private Mock<IEditService> mockService;
    private DefaultHttpContext httpContext;
    private TempDataDictionary tempData;

    public EditControllerTest()
    {
        mockService = new Mock<IEditService>();
        httpContext = new DefaultHttpContext();
        tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
    }
    // 正常系：データありのキー値を指定
    [Fact]
    public void IndexTest01()
    {
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.GetDiary(1)).Returns(editViewModel);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index("1");

        var actionResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditViewModel>(actionResult.ViewData.Model);
        Assert.Null(controller.TempData["notification"]);

        controller.Dispose();

    }

    // 準正常系：データなしのキーを指定
    [Fact]
    public void IndexTest02()
    {
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.GetDiary(1)).Returns(editViewModel);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index("2");

        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        var model = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Equal("Menu", actionResult.ControllerName);
        Assert.Equal("日記が見つかりませんでした。", controller.TempData["notification"]);

        controller.Dispose();

    }

    // 異常系：キー値が空文字
    [Fact]
    public void IndexTest03()
    {
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index(string.Empty);

        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        var model = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Equal("Menu", actionResult.ControllerName);
        Assert.Equal("日記が見つかりませんでした。", controller.TempData["notification"]);

        controller.Dispose();

    }

    // 異常系：キー値がnull
    [Fact]
    public void IndexTest04()
    {
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index(null);

        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        var model = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Equal("Menu", actionResult.ControllerName);
        Assert.Equal("日記が見つかりませんでした。", controller.TempData["notification"]);

        controller.Dispose();
    }

    // 正常系：データ更新成功
    [Fact]
    public void EditTest001()
    {
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.UpdateDiary(editViewModel)).Returns(true);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Edit(editViewModel);

        var model = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", model.ActionName);
        Assert.Equal("Refer", model.ControllerName);
        Assert.Null(controller.TempData["notification"]);

        controller.Dispose();
    }

    // 異常系：バリデーションエラー
    [Fact]
    public void EditTest002()
    {
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.UpdateDiary(editViewModel)).Returns(true);

        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;
        controller.ModelState.AddModelError("key", "ERROR");
        var result = controller.Edit(editViewModel);
        var model = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", model.ViewName);
        controller.Dispose();
    }

    // 異常系：データ更新失敗
    [Fact]
    public void EditTest003()
    {
        var editViewModel = new EditViewModel() { Id = "2", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.UpdateDiary(editViewModel)).Returns(false);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Edit(editViewModel);

        var model = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", model.ActionName);
        Assert.Equal("Menu", model.ControllerName);
        Assert.Equal("更新できませんでした。", controller.TempData["notification"]);

        controller.Dispose();
    }

    // 正常系：データ削除成功
    [Fact]
    public void DeleteTest001()
    {
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.DeleteDiary(1)).Returns(true);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Delete(editViewModel);

        var model = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", model.ActionName);
        Assert.Equal("Menu", model.ControllerName);
        Assert.Equal("削除しました。", controller.TempData["notification"]);

        controller.Dispose();
    }

    // 異常系：ID範囲外
    [Fact]
    public void DeleteTest002()
    {
        var editViewModel = new EditViewModel() { Id = "16777216", Title = string.Empty, Content = string.Empty };
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Headers["test"] = "test";

        var result = controller.Delete(editViewModel);

        var model = Assert.IsType<ViewResult>(result);
        Assert.Equal("Views/Shared/Error.cshtml", model.ViewName);

        controller.Dispose();
    }

    // 異常系：データ削除失敗
    [Fact]
    public void DeleteTest003()
    {
        var editViewModel = new EditViewModel() { Id = "2", Title = "たいとる更新（テスト）", Content = "ほんぶん更新（てすと）" };
        mockService.Setup(x => x.DeleteDiary(2)).Returns(false);
        var controller = new EditController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Delete(editViewModel);

        var model = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", model.ActionName);
        Assert.Equal("Menu", model.ControllerName);
        Assert.Equal("すでに削除されています。", controller.TempData["notification"]);

        controller.Dispose();
    }




}