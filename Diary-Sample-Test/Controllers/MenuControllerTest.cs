// -----------------------------------------------------------------------
// <copyright file="MenuControllerTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Controllers;
using Diary_Sample.Models;
using Diary_Sample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace Diary_Sample_Test.Controllers;

public class MenuControllerTest
{
    private readonly Mock<ILogger<MenuController>> mockLogger;
    private readonly Mock<IMenuService> mockService;
    private readonly DefaultHttpContext httpContext;
    private readonly TempDataDictionary tempData;

    public MenuControllerTest()
    {
        mockLogger = new Mock<ILogger<MenuController>>();
        mockService = new Mock<IMenuService>();
        httpContext = new DefaultHttpContext();
        mockService.Setup(x => x.Index(string.Empty)).Returns(new MenuViewModel(new List<DiaryRow>(), 1, 1, string.Empty));
        mockService.Setup(x => x.Paging(1)).Returns(new MenuViewModel(new List<DiaryRow>(), 1, 1, string.Empty));
        tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
    }
    [Fact]
    public void IndexTest()
    {
        MenuController controller = new MenuController(mockLogger.Object, mockService.Object)
        {
            TempData = tempData,
        };

        // Indexメソッドのテスト
        IActionResult result = controller.Index();

        // 返却値の型を確認
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<MenuViewModel>(viewResult.ViewData.Model);

        controller.Dispose();
    }
    [Fact]
    public void PagingTest()
    {
        MenuController controller = new MenuController(mockLogger.Object, mockService.Object)
        {
            TempData = tempData,
        };

        // Pagingメソッドのテスト
        IActionResult result = controller.Paging(1);

        // 返却値の型を確認
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<MenuViewModel>(viewResult.ViewData.Model);

        controller.Dispose();
    }
    [Fact]
    public void NewEntryTest()
    {
        MenuController controller = new MenuController(mockLogger.Object, mockService.Object)
        {
            TempData = tempData,
        };

        // NewEntryメソッドのテスト
        IActionResult result = controller.NewEntry();

        // 遷移先の確認
        RedirectToActionResult actionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("index", actionResult.ActionName);
        Assert.Equal("Create", actionResult.ControllerName);

        controller.Dispose();
    }
}