// -----------------------------------------------------------------------
// <copyright file="IEditService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;

namespace Diary_Sample.Services;

public interface IEditService
{
    public EditViewModel? GetDiary(int id);
    public bool UpdateDiary(EditViewModel editViewModel);
    public bool DeleteDiary(int id);
}