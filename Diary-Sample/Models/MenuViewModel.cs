// -----------------------------------------------------------------------
// <copyright file="MenuViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class MenuViewModel
{
    public const int PageCount = 5;

    public MenuViewModel(List<DiaryRow> diaryList, int totalNumber, int nowPage, string notification)
    {
        DiaryList = diaryList;
        Page = new PageViewModel(PageCount, totalNumber, nowPage);
        Notification = notification;
    }
    // 日記一覧（1ページ分）
    public List<DiaryRow> DiaryList { get; } = new List<DiaryRow>();
    // ページ情報
    public PageViewModel Page { get; }
    // 通知
    public string Notification { get; set; } = string.Empty;
}