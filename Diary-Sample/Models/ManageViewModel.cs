// -----------------------------------------------------------------------
// <copyright file="ManageViewModel.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Common;
using Microsoft.AspNetCore.Identity;
using static Diary_Sample.Common.ResultType;

namespace Diary_Sample.Models;

public class ManageViewModel
{
    public const int PageCount = 5;

    // 表示するページを指定で初期化する
    public ManageViewModel(UserManager<IdentityUser> userManager, int nowPage)
    {
        AllUsers = getAllUsers(userManager);
        Users = getPageUsers(AllUsers, nowPage);
        Page = new PageViewModel(PageCount, AllUsers.Count, nowPage);
    }
    // 指定したユーザが表示されるページを検索したうえで初期化する
    public ManageViewModel(UserManager<IdentityUser> userManager, IdentityUser targetUser)
        : this(userManager, nowPage: getNowPage(userManager, targetUser))
    {
    }

    // 全ユーザ一覧
    public List<UserViewModel> AllUsers { get; } = new List<UserViewModel>();
    // ユーザ一覧
    public List<UserViewModel> Users { get; } = new List<UserViewModel>();
    // ページ管理
    public PageViewModel Page { get; }
    // 通知
    public string Notification { get; set; } = string.Empty;
    // 通知の種類
    public ResultType NotificationType { get; set; } = None;
    // UserMangerから全ユーザを取得する
    private static List<UserViewModel> getAllUsers(UserManager<IdentityUser> userManager)
    {
        using (UserManager<IdentityUser>? um = userManager ?? throw new ArgumentNullException(nameof(userManager)))
        {
            return (from user in um.Users orderby user.Email select new UserViewModel(user)).ToList<UserViewModel>();
        }
    }
    // 指定ページのユーザリストを取得する
    private static List<UserViewModel> getPageUsers(List<UserViewModel> allUsers, int page)
    {
        return allUsers.Where((x, i) => (page - 1) * PageCount <= i && i < page * PageCount)
                       .ToList<UserViewModel>();
    }

    // 指定のユーザが表示されるページ番号を取得する
    private static int getNowPage(UserManager<IdentityUser> userManager, IdentityUser targetUser)
    {
        return (getAllUsers(userManager)
                       .Select((user, index) => new { user, index })
                       .Where(x => targetUser.Id == x.user.Id)
                       .Select(x => x.index).First() / PageCount) + 1;
    }
}