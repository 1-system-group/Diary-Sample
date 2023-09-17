// -----------------------------------------------------------------------
// <copyright file="ICreateService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;

namespace Diary_Sample.Services;

public interface ICreateService
{
    public CreateViewModel Index();
    public bool Create(CreateViewModel model);
}