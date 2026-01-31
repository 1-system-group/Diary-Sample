// -----------------------------------------------------------------------
// <copyright file="IForgotPasswordApiController.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Diary_Sample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diary_Sample.Controllers;

public interface IForgotPasswordApiController
{
    /// <summary>
    /// パスワードリセットメールを送信する
    /// </summary>
    /// <remarks>
    /// 登録済みメールアドレスにパスワードリセット用のリンクを送信する
    /// </remarks>
    /// <param name="model">パスワードリセット要求モデル</param>
    /// <returns>処理結果</returns>
    /// <response code="200">OK メール送信完了</response>
    /// <response code="400">NG 入力内容不正</response>
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
    public Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordModel model);
}