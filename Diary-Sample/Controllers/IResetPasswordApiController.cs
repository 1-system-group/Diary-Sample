// -----------------------------------------------------------------------
// <copyright file="IResetPasswordApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers;

public interface IResetPasswordApiController
{
    /// <summary>
    /// パスワードをリセットする
    /// </summary>
    /// <remarks>
    /// 受け取ったコードと新しいパスワードでパスワードをリセットする
    /// </remarks>
    /// <param name="model">パスワードリセットモデル</param>
    /// <returns>処理結果</returns>
    /// <response code="200">OK パスワードリセット完了</response>
    /// <response code="400">NG 入力内容不正</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> ResetPassword([FromBody] ResetPasswordModel model);
}