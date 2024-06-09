// -----------------------------------------------------------------------
// <copyright file="UserAdminAccountDeleteViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class UserAdminAccountDeleteViewModel
{
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "パスワードは必須です。")]
    [DataType(DataType.Password)]
    [Display(Name = "パスワード")]
    public string Password { get; set; } = string.Empty;

    public string Notification { get; set; } = string.Empty;

    public bool UpdateResult { get; set; } = true;

}