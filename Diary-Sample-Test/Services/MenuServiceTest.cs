// -----------------------------------------------------------------------
// <copyright file="MenuServiceTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Models;
using Diary_Sample.Repositories;
using Diary_Sample.Services;
using Microsoft.Extensions.Logging;

namespace Diary_Sample_Test.Services;

public class MenuServiceTest
{
    private readonly Mock<IDiaryRepository> mockRepository;
    private readonly MenuService service;
    public MenuServiceTest()
    {
        mockRepository = new Mock<IDiaryRepository>();
        service = new MenuService(
            new Mock<ILogger<MenuService>>().Object,
            new SharedService(new Mock<ILogger<ISharedService>>().Object, mockRepository.Object));
    }
    [Fact]
    public void IndexTest1()
    {
        // Diaryが0件の場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount)).Returns(new List<Diary>());
        mockRepository.Setup(x => x.readCount()).Returns(0);

        // Indexメソッドのテスト
        MenuViewModel result = service.Index("test");

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(0, model.Page.TotalNumber);
        Assert.Equal(0, model.Page.TotalPageNumber);
        Assert.Equal("test", model.Notification);
        Assert.Empty(model.DiaryList);
    }
    [Fact]
    public void IndexTest2()
    {
        // Diaryが1件の場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount))
            .Returns(Enumerable.Range(1, 1).Select(x => new Diary("タイトル", "本文")).ToList());
        mockRepository.Setup(x => x.readCount()).Returns(1);

        // Indexメソッドのテスト
        MenuViewModel result = service.Index("test");

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(1, model.Page.TotalNumber);
        Assert.Equal(1, model.Page.TotalPageNumber);
        Assert.Equal("test", model.Notification);
        Assert.Single(model.DiaryList);
    }
    [Fact]
    public void IndexTest3()
    {
        // Diaryが5件の場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount))
            .Returns(Enumerable.Range(1, 5).Select(x => new Diary("タイトル", "本文")).ToList());
        mockRepository.Setup(x => x.readCount()).Returns(5);

        // Indexメソッドのテスト
        MenuViewModel result = service.Index("test");

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(5, model.Page.TotalNumber);
        Assert.Equal(1, model.Page.TotalPageNumber);
        Assert.Equal("test", model.Notification);
        Assert.Equal(5, model.DiaryList.Count);
    }
    [Fact]
    public void PagingTest1()
    {
        // 0件の1ページの場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount)).Returns(new List<Diary>());
        mockRepository.Setup(x => x.readCount()).Returns(0);

        // Pagingメソッドのテスト
        MenuViewModel result = service.Paging(1);

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(0, model.Page.TotalNumber);
        Assert.Equal(0, model.Page.TotalPageNumber);
        Assert.Empty(model.Notification);
        Assert.Empty(model.DiaryList);
    }
    [Fact]
    public void PagingTest2()
    {
        // 1件の1ページの場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount))
            .Returns(Enumerable.Range(1, 1).Select(x => new Diary("タイトル", "本文")).ToList());
        mockRepository.Setup(x => x.readCount()).Returns(1);

        // Pagingメソッドのテスト
        MenuViewModel result = service.Paging(1);

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(1, model.Page.TotalNumber);
        Assert.Equal(1, model.Page.TotalPageNumber);
        Assert.Empty(model.Notification);
        Assert.Single(model.DiaryList);
    }
    [Fact]
    public void PagingTest3()
    {
        // 6件の1ページの場合
        mockRepository.Setup(x => x.read(1, MenuViewModel.PageCount))
            .Returns(Enumerable.Range(1, 5).Select(x => new Diary("タイトル", "本文")).ToList());
        mockRepository.Setup(x => x.readCount()).Returns(6);

        // Pagingメソッドのテスト
        MenuViewModel result = service.Paging(1);

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(6, model.Page.TotalNumber);
        Assert.Equal(2, model.Page.TotalPageNumber);
        Assert.Empty(model.Notification);
        Assert.Equal(5, model.DiaryList.Count);
    }
    [Fact]
    public void PagingTest4()
    {
        // 6件の2ページの場合
        mockRepository.Setup(x => x.read(2, MenuViewModel.PageCount))
            .Returns(Enumerable.Range(1, 1).Select(x => new Diary("タイトル", "本文")).ToList());
        mockRepository.Setup(x => x.readCount()).Returns(6);

        // Pagingメソッドのテスト
        MenuViewModel result = service.Paging(2);

        // 返却値の確認
        MenuViewModel model = Assert.IsType<MenuViewModel>(result);
        Assert.Equal(6, model.Page.TotalNumber);
        Assert.Equal(2, model.Page.TotalPageNumber);
        Assert.Empty(model.Notification);
        Assert.Single(model.DiaryList);
    }
}