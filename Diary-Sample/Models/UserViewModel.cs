// -----------------------------------------------------------------------
// <copyright file="UserViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Identity;
using static System.FormattableString;

namespace Diary_Sample.Models;

// ユーザ情報クラス
public class UserViewModel
{
    public UserViewModel(IdentityUser user)
    {
        user = user ?? throw new ArgumentNullException(nameof(user));
        Id = user.Id;
        UserName = user.UserName;
        Email = user.Email;
        EmailConfirmed = user.EmailConfirmed;
        PhoneNumber = user.PhoneNumber;
        LockOut = user.LockoutEnd != null ? true : false;
        LockoutEnd = user.LockoutEnd;
        AccessFailedCount = user.AccessFailedCount;

    }
    public string Id { get; } = string.Empty;
    public string UserName { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public bool EmailConfirmed { get; } = default;
    public string PhoneNumber { get; } = string.Empty;
    public DateTimeOffset? LockoutEnd { get; } = default;
    public bool LockOut { get; } = default;
    public int AccessFailedCount { get; } = default;

    // ロック解除時間（表示用）を取得する
    public string getDisplayLockOutDateTime()
    {
        TimeZoneInfo? jstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        DateTimeOffset localoutTime = LockoutEnd ?? default;
        return CurrentCulture(formattable: $"{TimeZoneInfo.ConvertTime(localoutTime, jstTimeZoneInfo).ToString("F", CultureInfo.CreateSpecificCulture("ja-JP"))}まで");
    }
}