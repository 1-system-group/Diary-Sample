// -----------------------------------------------------------------------
// <copyright file="IApiService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;

namespace Diary_Sample.Services;

public interface IApiService
{
    public List<DiaryRow> Lists(int page);
    public int Counts();
    public DiaryModel Diary(int id);
}