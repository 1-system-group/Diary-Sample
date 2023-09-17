// -----------------------------------------------------------------------
// <copyright file="MenuService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Services;

public class MenuService : IMenuService
{
    private const int InitialPage = 1;
    private readonly ILogger<MenuService> _logger;
    private readonly ISharedService _service;

    public MenuService(ILogger<MenuService> logger, ISharedService service)
    {
        _logger = logger;
        _service = service;
    }

    public MenuViewModel Index(string notification)
    {
        return createModel(InitialPage, notification);
    }

    public MenuViewModel Paging(int page)
    {
        return createModel(page, string.Empty);
    }

    private MenuViewModel createModel(int page, string notification)
    {
        return new MenuViewModel(
            _service.Lists(page, MenuViewModel.PageCount),
            _service.Counts(),
            page,
            notification);
    }
}