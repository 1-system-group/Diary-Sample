// -----------------------------------------------------------------------
// <copyright file="IDiaryRepository.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;

namespace Diary_Sample.Repositories;

public interface IDiaryRepository
{
    public bool create(Diary diary);
    public List<Diary> read(int page, int count);
    public int readCount();
    public List<Diary> Read(int id);
    public bool Update(Diary diary);
    public bool Delete(int id);
}