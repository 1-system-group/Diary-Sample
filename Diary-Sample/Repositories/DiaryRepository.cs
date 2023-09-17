// -----------------------------------------------------------------------
// <copyright file="DiaryRepository.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Entities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Diary_Sample.Repositories;

public class DiaryRepository : IDiaryRepository
{
    private readonly ILogger<DiaryRepository> _logger;
    private readonly DiarySampleContext _context;

    public DiaryRepository(ILogger<DiaryRepository> logger, DiarySampleContext context)
    {
        _logger = logger;
        _context = context;
    }

    public bool create(Diary diary)
    {
        try
        {
            _context.Add(diary);
            return _context.SaveChanges() > 0;
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw;
        }
    }
    public List<Diary> read(int page, int count)
    {
        try
        {
            return _context.diary
                .OrderByDescending(x => x.post_date)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList()
                .Select(x =>
                {
                    outputLog(x);
                    return x;
                })
                .ToList();
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw;
        }
    }

    public int readCount()
    {
        try
        {
            return _context.diary.Count();
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw;
        }
    }

    private void outputLog(Diary x)
    {
        // DBから取得した内容をログに出力
        _logger.LogInformation($"{x.id} {x.title} {x.content} {x.post_date} {x.update_date}");
    }

    public List<Diary> Read(int id)
    {
        var result = new List<Diary>();
        try
        {
            result = _context.diary.Where(x => x.id == id).ToList();
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw;
        }

        return result;
    }

    public bool Update(Diary diary)
    {
        try
        {
            var record = _context.diary.Single(x => x.id == diary.id);
            record.title = diary.title;
            record.content = diary.content;
            record.update_date = diary.update_date;
            _context.SaveChanges();

        }
        catch (InvalidOperationException ie)
        {
            _logger.LogError(ie.Message);
            _logger.LogError(ie.StackTrace);
            return false;
        }
        catch (MySqlException me)
        {
            _logger.LogError(me.Message);
            _logger.LogError(me.StackTrace);
            throw;
        }
        return true;

    }

    public bool Delete(int id)
    {
        try
        {
            var record = _context.diary.Single(x => x.id == id);
            _context.diary.Remove(record);
            _context.SaveChanges();

        }
        catch (InvalidOperationException ie)
        {
            _logger.LogError(ie.Message);
            _logger.LogError(ie.StackTrace);
            return false;
        }
        catch (MySqlException me)
        {
            _logger.LogError(me.Message);
            _logger.LogError(me.StackTrace);
            throw;
        }
        return true;
    }
}