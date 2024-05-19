// -----------------------------------------------------------------------
// <copyright file="ReferService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;

namespace Diary_Sample.Services;

public class ReferService : IReferService
{
    private readonly ISharedService _service;

    public ReferService(ISharedService service)
    {
        _service = service;
    }

    public ReferViewModel? GetDiary(int id)
    {
        DiaryModel diary = _service.Diary(id);
        if (diary == null)
        {
            return null;
        }

        ReferViewModel? viewModel = new ReferViewModel
        {
            Id = diary.Id,
            Title = diary.Title,
            Content = diary.Content,
        };

        return viewModel;
    }
}