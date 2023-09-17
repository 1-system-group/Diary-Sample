// -----------------------------------------------------------------------
// <copyright file="Diary.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Entities;

[Table("diary")]
public class Diary
{
    public Diary(string title, string content)
    {
        this.title = title;
        this.content = content;
        post_date = DateTime.Now;
        update_date = DateTime.Now;
    }

    public Diary(int id, string title, string content)
    {
        this.id = id;
        this.title = title;
        this.content = content;
        update_date = DateTime.Now;
    }

    public int id { get; set; }
    public string title { get; set; }
    public string content { get; set; }
    public DateTime post_date { get; set; }
    public DateTime update_date { get; set; }
}