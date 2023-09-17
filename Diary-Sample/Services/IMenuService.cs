// -----------------------------------------------------------------------
// <copyright file="IMenuService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;

namespace Diary_Sample.Services;

public interface IMenuService
{
    public MenuViewModel Index(string notification);
    public MenuViewModel Paging(int page);
}