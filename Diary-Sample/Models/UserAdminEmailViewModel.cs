// -----------------------------------------------------------------------
// <copyright file="UserAdminEmailViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public class UserAdminEmailViewModel
{
    [Required(ErrorMessage = "Eメールは必須です。")]
    [EmailAddress]
    [Display(Name = "Eメール")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "変更後Eメールは必須です。")]
    [EmailAddress]
    [Display(Name = "変更後Eメール")]
    public string NewEmail { get; set; } = string.Empty;

    public string Notification { get; set; } = string.Empty;

    public bool UpdateResult { get; set; } = true;

}