// -----------------------------------------------------------------------
// <copyright file="CommonUtilTest.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Common;

namespace Diary_Sample_Test.Common;

public class CommonUtilTest
{
    public CommonUtilTest()
    {
    }

    // 正常系：最小値、最大値
    [Fact]
    public void ConvAcceptIdTest001()
    {
        int ret1 = CommonUtil.ConvAcceptId("1");
        Assert.Equal(1, ret1);

        int ret2 = CommonUtil.ConvAcceptId("16777215");
        Assert.Equal(16777215, ret2);
    }

    // 異常系：null
    [Fact]
    public void ConvAcceptIdTest002()
    {
        int ret = CommonUtil.ConvAcceptId(null);
        Assert.Equal(-1, ret);
    }

    // 異常系：半角数字以外
    [Fact]
    public void ConvAcceptIdTest003()
    {
        int ret = CommonUtil.ConvAcceptId("１２３４５");
        Assert.Equal(-1, ret);
    }

    // 正常系：範囲外
    [Fact]
    public void ConvAcceptIdTest004()
    {
        int ret1 = CommonUtil.ConvAcceptId("0");
        Assert.Equal(-1, ret1);

        int ret2 = CommonUtil.ConvAcceptId("16777216");
        Assert.Equal(-1, ret2);
    }
}