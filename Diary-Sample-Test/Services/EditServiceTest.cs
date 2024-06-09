// -----------------------------------------------------------------------
// <copyright file="EditServiceTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Models;
using Diary_Sample.Repositories;
using Diary_Sample.Services;

namespace Diary_Sample_Test.Services;

public class EditServiceTest
{
    private readonly Mock<IDiaryRepository> mockRepository;

    public EditServiceTest()
    {
        mockRepository = new Mock<IDiaryRepository>();
    }

    // 正常系：レコード取得成功
    [Fact]
    public void GetDiaryTest001()
    {
        var diaryList = new List<Diary>();
        diaryList.Add(new Diary(1, "たいとる（てすと）", "ほんぶん（てすと）"));
        mockRepository.Setup(x => x.Read(1)).Returns(diaryList);
        var service = new EditService(mockRepository.Object);

        var result = service.GetDiary(1);
        var model = Assert.IsType<EditViewModel>(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("たいとる（てすと）", result.Title);
        Assert.Equal("ほんぶん（てすと）", result.Content);
    }

    // 異常系：レコード取得失敗
    [Fact]
    public void GetDiaryTest002()
    {
        var diaryList = new List<Diary>();
        mockRepository.Setup(x => x.Read(1)).Returns(diaryList);
        var service = new EditService(mockRepository.Object);

        var result = service.GetDiary(1);

        Assert.Null(result);
    }

    // 正常系：レコード更新成功
    [Fact]
    public void UpdateDiaryTest001()
    {
        var diary = new Diary(1, "たいとる更新その１（てすと）", "ほんぶん更新その１（てすと）");
        mockRepository.Setup(x => x.Update(It.Is<Diary>(x => x.id == 1))).Returns(true);
        var service = new EditService(mockRepository.Object);
        var editViewModel = new EditViewModel() { Id = "1", Title = "たいとる更新その１（てすと）", Content = "ほんぶん更新その１（てすと）" };

        var result = service.UpdateDiary(editViewModel);

        Assert.True(result);
    }


    // 正常系：レコード更新失敗
    [Fact]
    public void UpdateDiaryTest002()
    {
        var diary = new Diary(2, "たいとる更新その２（てすと）", "ほんぶん更新その２（てすと）");
        mockRepository.Setup(x => x.Update(It.Is<Diary>(x => x.id == 1))).Returns(false);
        var service = new EditService(mockRepository.Object);
        var editViewModel = new EditViewModel() { Id = "2", Title = "たいとる更新その２（てすと）", Content = "ほんぶん更新その２（てすと）" };

        var result = service.UpdateDiary(editViewModel);

        Assert.False(result);
    }

    // 正常系：レコード削除成功
    [Fact]
    public void DeleteDiaryTest001()
    {
        mockRepository.Setup(x => x.Delete(1)).Returns(true);
        var service = new EditService(mockRepository.Object);

        var result = service.DeleteDiary(1);

        Assert.True(result);
    }

    // 異常系：レコード削除成功
    [Fact]
    public void DeleteDiaryTest002()
    {
        mockRepository.Setup(x => x.Delete(2)).Returns(false);
        var service = new EditService(mockRepository.Object);

        var result = service.DeleteDiary(2);

        Assert.False(result);
    }
}