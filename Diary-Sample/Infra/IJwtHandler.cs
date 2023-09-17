// -----------------------------------------------------------------------
// <copyright file="IJwtHandler.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Diary_Sample.Infra;

public interface IJwtHandler
{
    public string GenerateEncodedToken(string userName, string deviceId, IList<string> roles);
}