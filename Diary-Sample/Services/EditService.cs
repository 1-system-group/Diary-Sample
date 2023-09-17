// -----------------------------------------------------------------------
// <copyright file="EditService.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Diary_Sample.Models;
using Diary_Sample.Repositories;

namespace Diary_Sample.Services;

public class EditService : IEditService
{
    private readonly IDiaryRepository _repository;

    public EditService(IDiaryRepository repository)
    {
        _repository = repository;
    }

    public EditViewModel? GetDiary(int id)
    {
        var diaryList = GetById(id);
        if (diaryList.Count == 0)
        {
            return null;
        }

        var diary = diaryList.First();
        EditViewModel? viewModel = new EditViewModel
        {
            Id = diary.id.ToString(),
            Title = diary.title,
            Content = diary.content,
        };

        return viewModel;
    }

    public bool UpdateDiary(EditViewModel editViewModel)
    {
        return UpdateById(int.Parse(editViewModel.Id), editViewModel.Title, editViewModel.Content);
    }

    public bool DeleteDiary(int id)
    {
        return DeleteById(id);
    }

    private List<Diary> GetById(int id)
    {
        var diaryList = _repository.Read(id);
        if (diaryList.Count != 1)
        {
            return new List<Diary>();
        }

        return diaryList;
    }

    private bool UpdateById(int id, string title, string content)
    {
        var diary = new Diary(id, title, content);
        return _repository.Update(diary);
    }

    private bool DeleteById(int id)
    {
        return _repository.Delete(id);
    }
}