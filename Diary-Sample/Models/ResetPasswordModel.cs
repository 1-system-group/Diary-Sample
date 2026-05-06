// -----------------------------------------------------------------------
// <copyright file="ResetPasswordModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace Diary_Sample.Models;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "Eメールは必須です。")]
    [EmailAddress(ErrorMessage = "Eメールアドレスの形式で入力してください。")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "パスワードは必須です。")]
    [StringLength(100, ErrorMessage = "{0}は{2}〜{1}文字で入力してください。", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "確認用パスワードは必須です。")]
    [Compare("Password", ErrorMessage = "パスワードと確認用パスワードが不一致です。")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "コードは必須です。")]
    public string Code { get; set; } = string.Empty;
}