// -----------------------------------------------------------------------
// <copyright file="CreateAccountViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Common;
using static Diary_Sample.Common.ResultType;

namespace Diary_Sample.Models;

public class CreateAccountViewModel
{
    // Eメールアドレス
    [Required(ErrorMessage = "Eメールは必須です。")]
    [EmailAddress(ErrorMessage = "Eメールが不正です")]
    [Display(Name = "Eメール")]
    public string Email { get; set; } = string.Empty;

    // ユーザ名
    // [Required(ErrorMessage = "ユーザ名は必須です。")]
    // [Display(Name = "ユーザ名")]
    // public string UserName { get; set; } = string.Empty;

    // パスワード
    [Required(ErrorMessage = "パスワードは必須です")]
    [DataType(DataType.Password)]
    public string Password1 { get; set; } = string.Empty;

    // パスワード（再入力）
    [Required(ErrorMessage = "パスワード（再入力）は必須です")]
    [DataType(DataType.Password)]
    public string Password2 { get; set; } = string.Empty;

    // 電話番号
    [Phone(ErrorMessage = "電話番号が不正です。")]
    [Display(Name = "電話番号")]
    public string? PhoneNumber { get; set; } = string.Empty;

    // 通知
    public string Notification { get; set; } = string.Empty;
    // 通知の種類
    public ResultType NotificationType { get; set; } = None;
}