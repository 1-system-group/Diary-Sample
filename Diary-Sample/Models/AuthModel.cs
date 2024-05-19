// -----------------------------------------------------------------------
// <copyright file="AuthModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class AuthModel
{
    // Eメールアドレス
    [Required(ErrorMessage = "Eメールは必須です。")]
    public string email { get; set; } = string.Empty;

    // パスワード
    [Required(ErrorMessage = "パスワードは必須です")]
    [DataType(DataType.Password)]
    public string password { get; set; } = string.Empty;

    // 端末ID
    [Required(ErrorMessage = "端末IDは必須です")]
    public string deviceId { get; set; } = string.Empty;
}