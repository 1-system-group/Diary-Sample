// -----------------------------------------------------------------------
// <copyright file="ForgotPasswordModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace Diary_Sample.Models;

public class ForgotPasswordModel
{
    [Required(ErrorMessage = "Eメールは必須です。")]
    [EmailAddress(ErrorMessage = "Eメールアドレスの形式で入力してください。")]
    public string Email { get; set; } = string.Empty;
}