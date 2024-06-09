// -----------------------------------------------------------------------
// <copyright file="CreateService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Models;
using Diary_Sample.Repositories;
using Microsoft.Extensions.Logging;

namespace Diary_Sample.Services;

public class CreateService : ICreateService
{
    private readonly ILogger<CreateService> _logger;
    private readonly IDiaryRepository _repository;

    public CreateService(ILogger<CreateService> logger, IDiaryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public CreateViewModel Index()
    {
        return new CreateViewModel();
    }

    public bool Create(CreateViewModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        return _repository.create(new Diary(model.Title, model.Content));
    }
}