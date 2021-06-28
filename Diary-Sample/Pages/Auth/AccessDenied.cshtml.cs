// -----------------------------------------------------------------------
// <copyright file="AccessDenied.cshtml.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Diary_Sample.Pages.Auth
{
    [AllowAnonymous]
    public class AccessDenied : PageModel
    {
    }
}