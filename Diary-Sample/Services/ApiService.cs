// -----------------------------------------------------------------------
// <copyright file="ApiService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Services;

public class ApiService : IApiService
{
    private const int PageCount = 10;
    private readonly ILogger<ApiService> _logger;
    private readonly ISharedService _service;

    public ApiService(ILogger<ApiService> logger, ISharedService service)
    {
        _logger = logger;
        _service = service;
    }
    public List<DiaryRow> Lists(int page) => _service.Lists(page, PageCount);
    public int Counts() => _service.Counts();
    public DiaryModel Diary(int id) => _service.Diary(id);
}