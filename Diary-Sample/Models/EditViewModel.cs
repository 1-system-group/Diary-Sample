// -----------------------------------------------------------------------
// <copyright file="EditViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class EditViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required(ErrorMessage = "タイトルは必須です。")]
    [MaxLength(200, ErrorMessage = "タイトルは{1}文字以内で入力してください。")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "本文は必須です。")]
    [MaxLength(4000, ErrorMessage = "本文は{1}文字以内で入力してください。")]
    public string Content { get; set; } = string.Empty;
}