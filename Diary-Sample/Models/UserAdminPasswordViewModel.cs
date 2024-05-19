// -----------------------------------------------------------------------
// <copyright file="UserAdminPasswordViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class UserAdminPasswordViewModel
{
    [Required(ErrorMessage = "現在のパスワードは必須です。")]
    [DataType(DataType.Password)]
    [Display(Name = "現在のパスワード")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "変更後のパスワードは必須です。")]
    [DataType(DataType.Password)]
    [Display(Name = "変更後のパスワード")]
    public string NewPassword1 { get; set; } = string.Empty;

    [Required(ErrorMessage = "変更後のパスワード（再入力）は必須です。")]
    [DataType(DataType.Password)]
    [Display(Name = "変更後のパスワード（再入力）")]
    public string NewPassword2 { get; set; } = string.Empty;

    public string Notification { get; set; } = string.Empty;

    public bool UpdateResult { get; set; } = true;

}