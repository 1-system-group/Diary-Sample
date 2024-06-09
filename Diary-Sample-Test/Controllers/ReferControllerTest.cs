// -----------------------------------------------------------------------
// <copyright file="ReferControllerTest.cs" company="1-system-group">
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

public class ReferControllerTest
{
    private Mock<IReferService> mockService;
    private DefaultHttpContext httpContext;
    private TempDataDictionary tempData;


    public ReferControllerTest()
    {
        mockService = new Mock<IReferService>();
        httpContext = new DefaultHttpContext();
        tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
    }

    // 正常系：データありのキー値を指定
    [Fact]
    public void IndexTest01()
    {
        mockService.Setup(x => x.GetDiary(1)).Returns(new ReferViewModel() { Id = "1", Title = "たいとる（てすと）", Content = "ほんぶん（てすと）" });
        var controller = new ReferController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index("1");

        var actionResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ReferViewModel>(actionResult.ViewData.Model);
        Assert.Null(controller.TempData["notification"]);

        controller.Dispose();

    }

    // 準正常系：データなしのキーを指定
    [Fact]
    public void IndexTest02()
    {
        var controller = new ReferController(mockService.Object);
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
        var controller = new ReferController(mockService.Object);
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
        var controller = new ReferController(mockService.Object);
        controller.TempData = tempData;

        var result = controller.Index(null);

        var actionResult = Assert.IsType<RedirectToActionResult>(result);
        var model = Assert.IsType<RedirectToActionResult>(actionResult);
        Assert.Equal("Index", actionResult.ActionName);
        Assert.Equal("Menu", actionResult.ControllerName);
        Assert.Equal("日記が見つかりませんでした。", controller.TempData["notification"]);

        controller.Dispose();

    }
}