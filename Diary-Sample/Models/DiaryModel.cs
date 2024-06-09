// -----------------------------------------------------------------------
// <copyright file="DiaryModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class DiaryModel
{
    public DiaryModel()
    {
    }
    public DiaryModel(int id, string title, string content)
    {
        Id = id.ToString();
        Title = title;
        Content = content;
    }

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}