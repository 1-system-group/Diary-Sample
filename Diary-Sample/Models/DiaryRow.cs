// -----------------------------------------------------------------------
// <copyright file="DiaryRow.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Models;

public readonly struct DiaryRow : IEquatable<DiaryRow>
{
    public string Id { get; }
    public string Title { get; }
    public string PostDate { get; }
    public DiaryRow(int id, string title, DateTime post_date)
    {
        CultureInfo cultureJp = CultureInfo.CreateSpecificCulture("ja-JP");

        Id = id.ToString("D", cultureJp);
        Title = title;
        PostDate = post_date.ToString("yyyy/MM/dd", cultureJp);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public bool Equals(DiaryRow other)
    {
        return Id == other.Id && Title == other.Title && PostDate == other.PostDate;
    }

    public static bool operator ==(DiaryRow left, DiaryRow right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DiaryRow left, DiaryRow right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        return obj is DiaryRow row && Equals(row);
    }
}