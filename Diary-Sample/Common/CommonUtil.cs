// -----------------------------------------------------------------------
// <copyright file="CommonUtil.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Common;

public class CommonUtil
{
    private const int MaxIdValue = 16777215;
    private const int MinIdValue = 1;
    public static int ConvAcceptId(string id)
    {
        if (id == null)
        {
            return -1;
        }
        int num;
        bool ret = int.TryParse(id, out num);
        if (!ret)
        {
            return -1;
        }
        if ((num < MinIdValue) || (num > MaxIdValue))
        {
            return -1;
        }

        return num;
    }
}