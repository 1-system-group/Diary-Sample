// -----------------------------------------------------------------------
// <copyright file="PageViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class PageViewModel
{
    public PageViewModel(int pageCount, int totalNumber, int nowPage)
    {
        PageCount = pageCount;
        TotalNumber = totalNumber;
        NowPage = nowPage;

        // 全件数からページ数を求める
        TotalPageNumber = TotalNumber % PageCount == 0
                ? TotalNumber / PageCount : (TotalNumber / PageCount) + 1;
    }
    // 1ページの件数
    public int PageCount { get; } = 1;
    // 全件数
    public int TotalNumber { get; } = 0;
    // 現在のページ
    public int NowPage { get; } = 1;
    // 全ページ数
    public int TotalPageNumber { get; } = 1;

    // 画面の行Noを取得する
    public int getDisplayNo(int index)
    {
        return index + 1 + ((NowPage - 1) * PageCount);
    }
    // 前ページがあるか
    public bool existPrevPage()
    {
        return NowPage > 1;
    }
    // 前ページを取得する
    public int getPrevPageNo()
    {
        return NowPage - 1;
    }
    // 現在ページか判定する
    public bool isNowPage(int index)
    {
        return NowPage == index;
    }
    // 次ページがあるか
    public bool existNextPage()
    {
        return NowPage < TotalPageNumber;
    }
    // 次ページを取得する
    public int getNextPageNo()
    {
        return NowPage + 1;
    }
}